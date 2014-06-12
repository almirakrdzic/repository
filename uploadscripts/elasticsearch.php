<?php

$SERVER = "http://10.102.216.70";

#$SERVER = "http://192.168.1.16";
$ES_SERVER = $SERVER.":9200";
$ES_BOOK_INDEX = "/books/book";

function curPageURL() {
 $pageURL = 'http';
 if ($_SERVER["HTTPS"] == "on") {$pageURL .= "s";}
 $pageURL .= "://";
 if ($_SERVER["SERVER_PORT"] != "80") {
  $pageURL .= $_SERVER["SERVER_NAME"].":".$_SERVER["SERVER_PORT"].$_SERVER["REQUEST_URI"];
 } else {
  $pageURL .= $_SERVER["SERVER_NAME"].$_SERVER["REQUEST_URI"];
 }
 return $pageURL;
}

function deleteDirectory($dir) {
    if (!file_exists($dir)) {
        return true;
    }

    if (!is_dir($dir)) {
        return unlink($dir);
    }

    foreach (scandir($dir) as $item) {
        if ($item == '.' || $item == '..') {
            continue;
        }

        if (!deleteDirectory($dir . DIRECTORY_SEPARATOR . $item)) {
            return false;
        }

    }

    return rmdir($dir);
}


abstract class API
{
    /**
     * Property: method
     * The HTTP method this request was made in, either GET, POST, PUT or DELETE
     */
    protected $method = '';
    /**
     * Property: endpoint
     * The Model requested in the URI. eg: /files
     */
    protected $endpoint = '';
    /**
     * Property: verb
     * An optional additional descriptor about the endpoint, used for things that can
     * not be handled by the basic methods. eg: /files/process
     */
    protected $verb = '';
    /**
     * Property: args
     * Any additional URI components after the endpoint and verb have been removed, in our
     * case, an integer ID for the resource. eg: /<endpoint>/<verb>/<arg0>/<arg1>
     * or /<endpoint>/<arg0>
     */
    protected $args = Array();
    /**
     * Property: file
     * Stores the input of the PUT request
     */
     protected $file = Null;
	 
	 // stores POST request
	 protected $data = Null;

    /**
     * Constructor: __construct
     * Allow for CORS, assemble and pre-process the data
     */
    public function __construct($request) {
        header("Access-Control-Allow-Orgin: *");
        header("Access-Control-Allow-Methods: *");
        header("Content-Type: application/json");

        $this->args = explode('/', rtrim($request, '/'));
        $this->endpoint = array_shift($this->args);
        if (array_key_exists(0, $this->args) && !is_numeric($this->args[0])) {
            $this->verb = array_shift($this->args);
        }

        $this->method = $_SERVER['REQUEST_METHOD'];
        if ($this->method == 'POST' && array_key_exists('HTTP_X_HTTP_METHOD', $_SERVER)) {
            if ($_SERVER['HTTP_X_HTTP_METHOD'] == 'DELETE') {
                $this->method = 'DELETE';
            } else if ($_SERVER['HTTP_X_HTTP_METHOD'] == 'PUT') {
                $this->method = 'PUT';
            } else {
                throw new Exception("Unexpected Header");
            }
        }

        switch($this->method) {
        case 'DELETE':
        case 'POST':
            $this->request = $this->_cleanInputs($_POST);
            $this->file = file_get_contents("php://input");
            break;
        case 'GET':
            $this->request = $this->_cleanInputs($_GET);
            break;
        case 'PUT':
            $this->request = $this->_cleanInputs($_GET);
            $this->file = file_get_contents("php://input");
            break;
        default:
            $this->_response('Invalid Method', 405);
            break;
        }
    }
	
	
    public function processAPI() {
        if ((int)method_exists($this, $this->endpoint) > 0) {
			try{
				$data = $this->{$this->endpoint}($this->args);
				return $this->_response($data);
			}catch(Exception $ex) {				
				return $this->_response(json_encode($ex->getMessage()), 404);
			}
        }
        return $this->_response("No Endpoint: $this->endpoint", 404);
    }

    private function _response($data, $status = 200) {
        header("HTTP/1.1 " . $status . " " . $this->_requestStatus($status));
        return json_encode($data);
    }

    private function _cleanInputs($data) {
        $clean_input = Array();
        if (is_array($data)) {
            foreach ($data as $k => $v) {
                $clean_input[$k] = $this->_cleanInputs($v);
            }
        } else {
            $clean_input = trim(strip_tags($data));
        }
        return $clean_input;
    }

    private function _requestStatus($code) {
        $status = array(  
            200 => 'OK',
            404 => 'Not Found',   
            405 => 'Method Not Allowed',
            500 => 'Internal Server Error',
        ); 
        return ($status[$code])?$status[$code]:$status[500]; 
    }
}

function POSTrequest($url, $data) {
	// use key 'http' even if you send the request to https://...
	$options = array(
		'http' => array(
			'header'  => "Content-type: application/x-www-form-urlencoded\r\n",
			'method'  => 'POST',
			//'content' => http_build_query($data),
			'content' => $data
		),
	);
	
	$context  = stream_context_create($options);
	$result = file_get_contents($url, false, $context);
	
	return $result;
}

function ESsearch($query, $private=false){
	global $ES_SERVER, $ES_BOOK_INDEX, $SERVER;

	$THIS_SERVICE = "http://".$_SERVER['HTTP_HOST'].$_SERVER['SCRIPT_NAME']."?request=";
	$THIS_SERVER = "http://".$_SERVER['HTTP_HOST'].$_SERVER['SCRIPT_NAME']."/elasticservice.php?request=getbook&id=";
	
	$ES_LINK = $ES_SERVER.$ES_BOOK_INDEX."/_search";
	
	$strs = explode(":", $query);
	
	if( count($strs) > 1 ) {
		$term = (string)$strs[0];
		unset($strs[0]);
		$query = implode($strs);
			
		$ES_QUERY = '{"fields":["isbn", "filename", "title", "authors", "publisher"], "query":{"term": {"'.$term.'":"'.addslashes($query).'"}}, "highlight":{"fields":{"keywords":{}}}}';

	}else {
		$ES_QUERY = '{"fields":["isbn", "filename", "title", "authors", "publisher"], "query":{"query_string":{"query":"'.addslashes($query).'"}}, "highlight":{"fields":{"keywords":{}}}}';
	}

	#$ES_QUERY = '{"fields":["isbn", "filename", "title", "authors", "publisher", "keywords"], "query":{"query_string":{"query":"'.addslashes($query).'"}}, "highlight":{"fields":{"keywords":{}}}}';
			
	//echo $ES_LINK." ".$ES_QUERY;
			
	$jsonresponse = POSTrequest($ES_LINK, $ES_QUERY);
			
	//echo $jsonresponse;
	$resp = json_decode($jsonresponse);
			
	$answ = array();
	$answ["took"] = $resp->took;
	$answ["timed_out"] = $resp->timed_out;
	$answ["num_results"] = $resp->hits->total;
			
	$results = array();
	foreach($resp->hits->hits as $hit) {
		$res = array();
		
		$res["id"] = $hit->_id;
		$res["download"] = "http://".$_SERVER['HTTP_HOST'].$_SERVER['SCRIPT_NAME']."?request=getbook&id=".$hit->_id;
		$res["imgurl"] = "http://".$_SERVER['HTTP_HOST'].$_SERVER['SCRIPT_NAME']."?request=getimage&id=".$hit->_id;
		$res["detailurl"] = "http://".$_SERVER['HTTP_HOST'].$_SERVER['SCRIPT_NAME']."?request=getdetail&id=".$hit->_id;
		$res["score"] = $hit->_score;
		$res["isbn"] = $hit->fields->isbn[0];
		if( $private ) {
			$res["filename"] = $hit->fields->filename[0];
		}
		
		$res["highlight"] = @$hit->highlight->keywords[0];
		
		/*if( isset($_GET['details']) )
		{
			$res["details"] = array();
			$res["details"]["title"] = "Naslov knjige";
			$res["details"]["subtitle"] = "Pod naslov knjige";
			$res["details"]["publicationDate"] = "22.2.1998";
			$res["details"]["publisher"] = "izdavac";
			$res["details"]["description"] = "Opis knjige";
			$res["details"]["institution"] = "IEEE";
		}*/
		
		array_push($results, $res);
	}
		
	$answ["results"] = $results;
	
	return $answ;
}

class MyAPI extends API
{
    protected $User;

    public function __construct($request, $origin) {
        parent::__construct($request);
    }
	
	protected function getdetail() {
		global $ES_SERVER, $ES_BOOK_INDEX;
		
		$id = $this->request["id"];
	
		$url = $ES_SERVER.$ES_BOOK_INDEX."/".$id."?fields=filename";
		$result = @file_get_contents($url);
		$result = json_decode($result);
		
		if( $result == NULL || isset($result->found) && $result->found === false )
			throw new Exception("No id found", 404);
		
		$ds = DIRECTORY_SEPARATOR;		
		$filename = $result->fields->filename[0];
		
		###########################################################################
		#$filename = "C:/Users/edin/Desktop/nwt/primjeri knjiga/04432245.ebook.zip";
		#$filename = "C:\\xampp\\htdocs\\nwt\\archive\\0201734842.ebook.zip";
		
		$basename = basename($filename);
		$tmpdir = sys_get_temp_dir();
		
		$zip = new ZipArchive;
		if( $zip->open($filename) === TRUE )
		{
			$out = $tmpdir.$ds.$basename.$ds;
			$zip->extractTo($out);
			$zip->close();
			
			$metapath = $out.$ds."custom".$ds."plain.xml";
			if( !file_exists($metapath) )
				throw new Exception("File not found", 404);
			
		} else {
			throw new Exception("Unable to open file", 404);
		}

		$metaxml = file_get_contents($metapath);
		
		$xml = new SimpleXMLElement($metaxml);
		
		$data = array();
		
		$data["download"] = "http://".$_SERVER['HTTP_HOST'].$_SERVER['SCRIPT_NAME']."?request=getbook&id=".$id;
		$data["imgurl"] = "http://".$_SERVER['HTTP_HOST'].$_SERVER['SCRIPT_NAME']."?request=getimage&id=".$id;
		$data["detailurl"] = "http://".$_SERVER['HTTP_HOST'].$_SERVER['SCRIPT_NAME']."?request=getdetail&id=".$id;
		
		$data["isbn"] = (string)($xml->xpath("//ISBN")[0]);
		$data["title"] = (string) ($xml->xpath("//Title")[0]);
		$data["description"] = (string) ($xml->xpath("//Content")[0]);
		$data["elasticid"] = (string) $this->request["id"];
		$data["numofpages"] = (string) ($xml->xpath("//NumberOfPages")[0]);
		$data["pubdate"] = (string) ($xml->xpath("//PublicationDate")[0]);
		$data["publisher"] = (string) ($xml->xpath("//Publisher")[0]);
		$data["institution"] = (string) ($xml->xpath("//Institution")[0]);
		
		deleteDirectory($out);
		
		return $data;
	}
	
	protected function getimage() {
		global $ES_SERVER, $ES_BOOK_INDEX;
	
		$id = $this->request["id"];
	
		$url = $ES_SERVER.$ES_BOOK_INDEX."/".$id."?fields=filename";
		$result = @file_get_contents($url);
		$result = json_decode($result);
		if( $result == NULL || isset($result->found) && $result->found === false )
			throw new Exception("No id found", 404);
		
		$ds = DIRECTORY_SEPARATOR;		
		$filename = $result->fields->filename[0];
		
		###########################################################################
		#$filename = "C:/Users/edin/Desktop/nwt/primjeri knjiga/04432245.ebook.zip";
		#$filename = "C:\\xampp\\htdocs\\nwt\\archive\\0201734842.ebook.zip";
		
		$basename = basename($filename);
		$tmpdir = sys_get_temp_dir();
		
		$zip = new ZipArchive;
		if( $zip->open($filename) === TRUE )
		{
			$out = $tmpdir.$ds.$basename.$ds;
			$zip->extractTo($out);
			$zip->close();
			
			$imagePath = $out.$ds."meta".$ds."cover.jpg";
			if( !file_exists($imagePath) )
				$imagePath = getcwd().$ds."image".$ds."imagenotavailable_320.jpg";
			
		} else {
			$imagePath = getcwd().$ds."image".$ds."imagenotavailable_320.jpg";
		}
		
		$file_extension = strtolower(substr(strrchr($imagePath,"."),1));

		switch( $file_extension ) {
			case "gif": $ctype="image/gif"; break;
			case "png": $ctype="image/png"; break;
			case "jpeg":
			case "jpg": $ctype="image/jpg"; break;
			default:
		}

		header('Content-type: ' . $ctype);
		readfile($imagePath);
			
		deleteDirectory($out);
			
		die;
	}
	
	protected function search() {
		
		if( !($this->method == 'GET') ) {
			throw new Exception("Only GET method allowed.");
		}
		
		try{
			$query = $this->request["query"];
			
			$answ = ESsearch($query);
			
			return $answ;
		}
		catch(Exception $e){
			throw new Exception("Query parameter not specified");
		}
	}
	
	protected function getbook() {
		if( !($this->method == 'GET') ) {
			throw new Exception("Only GET method allowed.");
		}
		
		global $ES_SERVER, $ES_BOOK_INDEX;
		
		$id = $_GET["id"];
		
		//header("Content-Type:application/octet-stream");
		$url = $ES_SERVER.$ES_BOOK_INDEX."/".$id."?fields=filename";
		$result = @file_get_contents($url);
		$result = json_decode($result);
		if( $result == NULL || isset($result->found) && $result->found === false )
			throw new Exception("No id found", 404);
			
		$filename = $result->fields->filename[0];
		###########################################################################
		#$filename = "C:/Users/edin/Desktop/nwt/primjeri knjiga/04432245.ebook.zip";
		#$filename = "C:\\xampp\\htdocs\\nwt\\archive\\0201734842.ebook.zip";
		
		if( !file_exists($filename) )
			throw new Exception("File not found", 404);
		
		header("Content-Type: application/octet-stream");
		header('Content-disposition: filename="'.basename($filename).'"');
		echo readfile($filename);
		
	}
 }
 
// Requests from the same server don't have a HTTP_ORIGIN header
if (!array_key_exists('HTTP_ORIGIN', $_SERVER)) {
    $_SERVER['HTTP_ORIGIN'] = $_SERVER['SERVER_NAME'];
}

try {
    $API = new MyAPI($_REQUEST['request'], $_SERVER['HTTP_ORIGIN']);
    echo $API->processAPI();
} catch (Exception $e) {
    echo json_encode(Array('error' => $e->getMessage()));
}

?>
