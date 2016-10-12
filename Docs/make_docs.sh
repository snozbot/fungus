#!/bin/bash
rm -rf ../Build/Docs
cp ../Assets/Fungus/Docs/CHANGELOG.txt fungus_docs/change_log.md
/Applications/Doxygen.app/Contents/Resources/doxygen ./Doxyfile
cp -R ../Build/Docs/* ../../snozbot.github.io