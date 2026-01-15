# Script
Get-Date
Write-Host "Hello World!"
Get-Process | Format-Table
"cmdlet count:"
Get-Command -CommandType Cmdlet | Measure-Object | Select-Object -ExpandProperty Count
"function count:"
Get-Command -CommandType Function | Measure-Object | Select-Object -ExpandProperty Count
"Variables:"
Get-Variable
"Environment Variables:"
Get-ChildItem env:\ | Format-Table -Property Name, Value