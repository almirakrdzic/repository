import zipfile, sys, re, os
from zipfile import ZipFile
import json
import urllib2
import xml.etree.ElementTree as ET

#SERVER_ADDRESS = "http://192.168.1.13:9200/books/book"
SERVER_ADDRESS = "http://localhost:9200/books/book"

APP_SERVER_ADDRESS = "http://192.168.1.13:4416/api/resource/insertbook"

filename = sys.argv[1]

def getfile(infile):
	with ZipFile(filename, "r") as zipfile:
		xml = zipfile.read(infile)
		return xml
		
try:
	xml=getfile('custom\\plain.xml')
except:
	sys.exit()
	
keywords=getfile("keywords.txt")
keywords = re.sub(r'[^(\x20-\x7F)]+', " ", keywords)
#print keywords


def get(path):
	try:
		return root.findall(path)[0].text
	except:
		return ""

root=ET.fromstring(xml)

isbn=get(".//ISBN")
title=get(".//Title")
desc=get(".//Content")
authors=get(".//Authors")

data = json.dumps({"isbn":isbn, "title":title, "description":desc, "authors":authors, "keywords":keywords, "filename": os.path.realpath(filename)})

req = urllib2.Request(SERVER_ADDRESS)
req.add_header('Content-Type', 'application/json')

response = urllib2.urlopen(req, data)

#print isbn, title, desc
resp = response.read()

json.loads(resp)



