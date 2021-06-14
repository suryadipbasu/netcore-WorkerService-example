$mypath = $MyInvocation.MyCommand.Path
$mypath = Split-Path $mypath

cd $mypath\PerformArithmaticOperations
dotnet clean
dotnet build
dotnet publish -c Release -r win-x64 --output "C:\temp\ArithmaticOperationsService" PerformArithmaticOperations.sln

sc.exe create ArithmaticOperationsService binpath="C:\temp\ArithmaticOperationsService\PerformArithmaticOperations.exe" start=auto