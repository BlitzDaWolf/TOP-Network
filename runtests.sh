#!/bin/bash

rm -rf ./TestResults/

# dotnet restore
dotnet test --collect:"XPlat Code Coverage" --results-directory:"./TestResults"
reportgenerator -reports:"./TestResults/*/coverage.cobertura.xml" -targetdir:"./covrageReport/" -historydir:".//covrageReport/covragehistory"