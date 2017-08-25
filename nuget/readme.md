Bevor das publish Script aufgerufen werden kann, muss
der NuGet API Key als Environment Variable abgelegt werden:

export nugetApiKey=12345678-123456-12345678

Anschließend kann das Script gestartet werden mit

./publish

—

Alternative (und so in `publish` derzeit eingetragen):

Der NuGet API Key wird in einer Datei mit "secrets" hinterlegt, deren Zeilen den Aufbau `name "=" value` haben. (Leerzeichen vor/nach/zwischen diesen Zeilenelementen vermeiden!)

Derzeit wird als Name für den Key `nuget_org_api_key` erwartet.

Der absolute filename der Datei mit diesem Geheimnis muss in der environment-Variablen SECRET_STORE hinterlegt sein.
