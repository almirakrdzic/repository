find . -type f -name "*.ebook.zip" | while read filename; do

	dirname=`dirname "$filename"`
	absdirname=$(cd $dirname; pwd)
	
	absfilename=$absdirname/`basename "$filename"`
	
	rawname=`basename "${filename%.*}"`
	echo $absfilename
	id=$(echo "$rawname" | sed -r 's/([0-9]*)(.*)/\1/g')
	
	output=`unzip -qq -p "$absfilename" keywords.txt`
	#coded=`echo "$output" | perl -MMIME::Base64 -ne 'print encode_base64($_)'`
	
	#output=`echo $output | sed -i 's/[\d128-\d255]//g'`
	
	output=`echo "$output" | perl -p -e '/[^(\x20-\x7F)]*/'`
	#output=`echo "$output" | iconv -f utf-8 -t us-ascii//TRANSLIT`
	$(echo "$output" > json.file)
	json="{\"filename\":\"${absfilename}\", \"isbn\":\"${id}\", \"keywords\":\"${output}\"}"
	
	#$(echo "$json" > `basename "$id"`.txt)
	#$(echo "$json" > json.file)
	
	#sed -i 's/[\d128-\d255]//g' json.file
	#perl -i.bk -pe 's/[^[:ascii:]]//g;' json.file
	
	#server="192.168.1.17:9200"
	#cmnd="curl -s -XPOST \"$server/books/book\" -d '$json'"
	$(curl -sf -XPOST "http://localhost:9200/books/book/" -d @json.file 2&>1 > /dev/null)
	
	echo $?
	
	#echo `pwd -d`

	#echo `dirname $filename`/`basename "$filename"`
	
	extension="${filename##*.}"
	#echo $extension

done
