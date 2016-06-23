import os

notice = (  "/**\n"
			" * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).\n"
			" * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)\n"
			" */\n\n")

exclude_paths = [
	"Assets/Fungus/Thirdparty/CSVParser/",
	"Assets/Fungus/Thirdparty/iTween/",
	"Assets/Fungus/Thirdparty/LeanTween/",
	"Assets/Fungus/Thirdparty/Microsoft/",
	"Assets/Fungus/Thirdparty/Usfxr/Editor/",
	"Assets/Fungus/Thirdparty/Usfxr/Scripts/",
	"Assets/UnityTestTools/"
]

for root, dirs, files in os.walk("Assets"):
	for file in files:
		if file.endswith(".cs"):

			exclude = False

			file_path = os.path.join(root, file)
			for exclude_path in exclude_paths:

				if file_path.startswith(exclude_path):
					exclude = True
					break

			if not exclude:
				data = notice + open(file_path).read()
				print data
				open(file_path, "w").write(data)
