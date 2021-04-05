Das sind mal die zwei vergleichbaren Versionen. Beide testen mit variabler Länge (max. 4) und 'ZZZZ' als gesuchtes Passwort.
Die parallele Version war bei mir mit 6 Threads am besten, ist aber wohl HW-abhängig. Die Anzahl der Threads kannst du aber im Code einfach unter "T_NUM" ändern.
Bei mir hat außerdem das gröbere Chunken (also nicht jeder Buchstabe einzeln) ein kleines bisschen besser performt. Deswegen läuft der Code jetzt so.
Kann man aber bei Bedarf umstellen.

Kompilieren:
  
  g++-10 pw_cracker_sequential.cpp sha1.cpp -o [Name für das Programm]
  
  g++-10 -fopenmp pw_cracker_parallel.cpp sha1.cpp -o [Name für das Programm]
  
So funktionierts zumindest bei mir. Falls was nicht klappt einfach Bescheid geben.
