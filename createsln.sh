#!/bin/bash

CURRENT_PATH=$PWD
echo $CURRENT_PATH

if [[ -f "$PWD/TOP-Network.sln" ]]; then
    rm $PWD/TOP-Network.sln
fi

if [[ ! $RECORD ]]; then
    if [[ $1 == "local" ]]; then
        export RECORD="$PWD/../TOP-Records/TOP-Records"
        rm -rf ./TOP-Records
    fi
    if [[ $1 == "remote" ]]; then
        export RECORD="$PWD/TOP-Records/TOP-Records"
        rm -rf ./TOP-Records
        git clone git@github.com:BlitzDaWolf/TOP-Records.git
    fi
fi
if [[ ! $UTIL ]]; then
    if [[ $1 == "local" ]]; then
        export UTIL="$PWD/../TOP_Utils/TOP_Utils"
        rm -rf ./TOP-Records
    fi
    if [[ $1 == "remote" ]]; then
        export UTIL="$PWD/TOP_Utils/TOP_Utils/"
        rm -rf ./TOP_Utils
        git clone git@github.com:BlitzDaWolf/TOP_Utils.git
    fi
fi

cd "$RECORD/.."
echo "Running: $PWD/createsln.sh $1"
./createsln.sh $1
echo ""
cd "$UTIL/.."
echo "Running: $PWD/createsln.sh $1"
./createsln.sh $1
echo ""
cd $CURRENT_PATH

cd ./TOP-Network
cp TOP-Network.csproj-tmp TOP-Network.csproj
dotnet add reference "$RECORD/TOP-Records.csproj"
dotnet add reference "$UTIL/TOP_Utils.csproj"
cd $CURRENT_PATH

dotnet new sln

dotnet sln add ./TOP-Network
dotnet sln add $RECORD --solution-folder Libary
dotnet sln add $UTIL --solution-folder Libary
