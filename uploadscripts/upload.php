<?php

$ELASTIC_IP = "http://10.102.216.70:9200/";
$ELASTIC_INDEX = "books/";
$ELASTIC_TYPE = "book";

$PDF_TO_TEXT = "pdftotext";

$ds = DIRECTORY_SEPARATOR;  //1
 
$storeFolder = 'uploads';   //2

function find_isbn($str, &$result)
{
    $regex = '/\b(?:ISBN(?:: ?| ))?((?:97[89])?\d{9}[\dx])\b/i';

    if (preg_match($regex, str_replace('-', '', $str), $matches)) {
        $ret = (10 === strlen($matches[1]))
            ? 1   // ISBN-10
            : 2;  // ISBN-13
		$result = $matches[1];
		return $ret;
    }
    return FALSE; // No valid ISBN found
}
	
function convert_to_txt($filename)
{
	global $PDF_TO_TEXT;
	$pdftotext = $PDF_TO_TEXT;
	$command = $pdftotext." -f 0 -l 15 \"".realpath($filename)."\"";
	shell_exec($command);
}

function extract_isbn($filename)
{
	$filename = realpath($filename);
	$dirname = dirname($filename);
	$basename = basename($filename, ".pdf");
	$ext = pathinfo($filename, PATHINFO_EXTENSION);
	
	$filename = $dirname.DIRECTORY_SEPARATOR.$basename.".txt";
	$content = file_get_contents($filename);
	
	$isbn = "";
	$type = find_isbn($content, $isbn);
	#echo "ISBN: ".$isbn;
	
	return $type != FALSE ? $isbn : FALSE;
}

function get_metadata_from_google($isbn)
{
	return file_get_contents("https://www.googleapis.com/books/v1/volumes?q=isbn:".$isbn);
}

function get_metadata($isbn)
{
	return get_metadata_from_google($isbn);
}

function save_metadata($filename, $metadata)
{
	if( !$metadata )
		return;
	
	file_put_contents($filename, $metadata);
}

function stemname($filename)
{
	$filename = realpath($filename);
	$ext = pathinfo($filename, PATHINFO_EXTENSION);
	
	return basename($filename, ".".$ext);
}

function send_to_elastic($data) {
	global $ELASTIC_IP, $ELASTIC_INDEX, $ELASTIC_TYPE;
	
	$url = $ELASTIC_IP.$ELASTIC_INDEX.$ELASTIC_TYPE;

	// Create the context for the request
	$context = stream_context_create(array(
		'http' => array(
			// http://www.php.net/manual/en/context.http.php
			'method' => 'POST',
			'header' => "Content-Type: application/json\r\n",
			'content' => json_encode($data)
		)
	));
	
	// Send the request
	$response = file_get_contents($url, FALSE, $context);

	return $response;
}

function extract_metadata($isbn, $filepath, $keywords, $metadata){
	
	$data = json_decode($metadata);
	
	$item = $data->items[0];
	$volumeInfo = $item->volumeInfo;
	
	$authors = "";
	foreach($volumeInfo->authors as $author)
		$authors .= $author.";";
	
	$resp = array();
	$resp["authors"] = $authors;
	$resp["publisher"] = $volumeInfo->publisher;
	$resp["publishedDate"] = $volumeInfo->publishedDate;
	$resp["description"] = $volumeInfo->description;
	$resp["pageCount"] = $volumeInfo->pageCount;
	$resp["title"] = $volumeInfo->title;
	$resp["isbn"] = $isbn;
	$resp["filepath"] = $filepath;
	$resp["file"] = $keywords;
	
	return $resp;
}

function to_xml($data){
	$book = new SimpleXMLElement('<Book/>');
	
	$itemattrs = $book->addChild("Items")->addChild("Item")->addChild("ItemAttributes");
	$itemattrs->addChild("Author", $data["authors"]);
	$itemattrs->addChild("ISBN", $data["isbn"]);
	$itemattrs->addChild("NumberOfPages", $data["pageCount"]);
	$itemattrs->addChild("PublicationDate", $data["publishedDate"]);
	$itemattrs->addChild("Publisher", $data["publisher"]);
	$itemattrs->addChild("Title", $data["title"]);
	$itemattrs->addChild("Content", $data["description"]);	
	$itemattrs->addChild("Institution", $data["publisher"]);	
	
	return $book->asXML();
}

if(isset($_GET["archive"])){

	$data = json_decode(file_get_contents('php://input'), true);
	$isbn = $data["isbn"];
	$stemname = $data["stemname"];
	$metadata = $data["metadata"];
	
	$metadata["filename"] = realpath("archive/".$isbn.".ebook.zip");
	
	$zip = new ZipArchive();
	if( $zip->open("archive/".$isbn.".ebook.zip") === TRUE )
	{
		$keywords = (string)file_get_contents('zip://'.$metadata["filename"].'#keywords.txt'); 
		$keywords = preg_replace('/[^(\x20-\x7F)]*/','', $keywords);
		$metadata["keywords"] = $keywords;
	
		send_to_elastic($metadata);
		$zip->addFromString("custom/plain.xml", to_xml($metadata));
		
		$zip->close();
		
		echo "success";
	}
	else {
		echo "Unable to open zip";
		http_response_code(500);
	}

	die;
}
	
if (!empty($_FILES)) {
     
	$tempFile = $_FILES['file']['tmp_name'];
	$targetPath = dirname( __FILE__ ) . $ds. $storeFolder . $ds; 
	$targetFile =  $targetPath.$_FILES['file']['name'];
 
    move_uploaded_file($tempFile, $targetFile);
	convert_to_txt($targetFile);
	$isbn = extract_isbn($targetFile);	
	$metadata = FALSE;
	
	$stemname = stemname($targetFile);
	
	$imagePath = "uploads/".$stemname.".jpg";
	exec('convert -resize 143x107 "'.$targetFile.'"[0] "'.$imagePath.'"');
	
	if( $isbn !== FALSE )
	{
		$metadata = get_metadata($isbn);
	}
	else
	{
		echo "ISBN and metadata not found";
		http_response_code(404);
		die;
	}
		
	if( $metadata !== FALSE )
	{
		//file_put_contents("uploads/".$stemname.".meta", $metadata);
	}
	
	$zip = new ZipArchive();
	if( $zip->open("archive/".$isbn.".ebook.zip", ZipArchive::CREATE) === TRUE )
	{		
		$out = array();
		$out["isbn"] = $isbn;
		$out["stemname"] = $stemname;
		$out["metadata"] = $metadata;
		
		$zip->addFile($targetFile, $stemname.".".pathinfo($targetFile, PATHINFO_EXTENSION));
		$zip->addFile("uploads/".$stemname.".txt", "keywords.txt");
	
		$keywordsFile = realpath("uploads/".$stemname.".txt");
		$keywords = (string)file_get_contents($keywordsFile);
		$keywords = preg_replace('/[^(\x20-\x7F)]*/','', $keywords);
		$data = extract_metadata($isbn, realpath("archive/".$isbn.".ebook.zip"), $keywords, $metadata);
		//@send_to_elastic($data);
		
		//$zip->addFromString("custom/plain.xml", to_xml($data));
		$zip->addFile($imagePath, "meta/cover.jpg");
		
		$zip->close();
		
		unset($data["file"]);
		unset($data["filepath"]);
		$out["metadata"] = $data;
		
		echo json_encode($out);
	}
	else
	{
		echo "Unable to open zip";
		http_response_code(500);
	}
	
	http_response_code(200);
}

?>

