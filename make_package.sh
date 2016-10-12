#!/bin/bash
FUNGUS_VERSION=v3.3.0

UNITY_PATH=/Applications/Unity_5_4_1/Unity.app/Contents/MacOS/Unity
DOXYGEN_PATH=/Applications/Doxygen.app/Contents/Resources/doxygen

FUNGUS_PROJECT_PATH=~/github/fungus
FUNGUS_TEST_RESULTS_PATH=${FUNGUS_PROJECT_PATH}/Build/TestResults
FUNGUS_PACKAGE_PATH=${FUNGUS_PROJECT_PATH}/Build/Package
FUNGUS_PACKAGE_FILE=Fungus_${FUNGUS_VERSION}.unitypackage
FUNGUS_DOCS_SOURCE_PATH=${FUNGUS_PROJECT_PATH}/Docs
FUNGUS_DOCS_BUILD_PATH=${FUNGUS_PROJECT_PATH}/Build/Docs

# Run integration tests
# mkdir -p ${FUNGUS_TEST_RESULTS_PATH}
# ${UNITY_PATH} -batchmode -quit -projectPath ${FUNGUS_PROJECT_PATH} -executeMethod UnityTest.Batch.RunIntegrationTests -testscenes=ControlAudioTest 

# Export fungus package
# mkdir -p {$FUNGUS_PACKAGE_PATH}
# ${UNITY_PATH} -batchmode -quit -projectPath ${FUNGUS_PROJECT_PATH} -exportPackage Assets/Fungus Assets/FungusExamples ${FUNGUS_PACKAGE_PATH}/${FUNGUS_PACKAGE_FILE}

# Generate documentation
#rm -rf ${FUNGUS_DOCS_BUILD_PATH}
#cp ./Assets/Fungus/Docs/CHANGELOG.txt ${FUNGUS_DOCS_SOURCE_PATH}/fungus_docs/change_log.md

#cd ${FUNGUS_DOCS_SOURCE_PATH}
#${DOXYGEN_PATH} ./Doxyfile
#cd ..

#cp -R ${FUNGUS_DOCS_BUILD_PATH}/* ../snozbot.github.io
