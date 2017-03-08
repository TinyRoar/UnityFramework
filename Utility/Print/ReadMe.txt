Tiny Roar UnityFramework
Print.cs

Für Änderungen:

1. Solution irgendwo entpacken außerhalb vom unity projekt
2. Drauf achten dass die Datei UnityEngine.dll richtig verlinkt ist:
	Solution Explorer -> References -> Add Reference.
	Framework -> Browse -> [Unity Ordner]/Editor/Data/Managed\UnityEngine.dll
	mit OK bestätigen
3. Änderung machen
4. Kompilieren (Tab Build -> Build Solution)
5. dll befindet sich im bin/Debug Ordner. Die dll und das zip ersetzen im Tiny Roar UnityFramework



Have Fun