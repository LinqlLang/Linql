$text = 'await states.Where(r => r.State_Code! === "MA").Where(r => r.Data!.Any(s => s.Value > 1)).ToListAsync();';

Sleep 1

$window = Get-Process | Where { $_.MainWindowTitle.Contains("Visual Studio Code") }

Write-host $window.MainWindowTitle;

$wshell = New-Object -ComObject wscript.shell;
$wshell.AppActivate($window.MainWindowTitle);
Write-Host 'click into app'
Sleep 2;

$text.ToCharArray().ForEach({
        $textToSend = $_ -replace "[+^%~()]", '{$0}'
        $wshell.SendKeys($textToSend);
        Sleep .25;
    });

