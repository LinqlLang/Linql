Get-Process | Where { $_.MainWindowTitle } | Select { $_.MainWindowTitle }