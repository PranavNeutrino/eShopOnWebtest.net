#!/bin/bash
export TERM=xterm
dotnet test tests/UnitTests/UnitTests.csproj --filter "FullyQualifiedName~OrderServiceTests" --verbosity normal --logger "trx;LogFileName=order_tests.trx" --results-directory /app/TestResults
echo "EXIT=$?"
