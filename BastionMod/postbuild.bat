copy /y "C:\Users\%USERNAME%\source\repos\BastionPublishing\BastionMod\bin\Release\BastionComics.dll" "C:\Program Files (x86)\Steam\steamapps\common\Sentinels of the Multiverse\mods\BastionPublishing"
copy /y "C:\Users\%USERNAME%\source\repos\BastionPublishing\Resources\manifest.json" "C:\Program Files (x86)\Steam\steamapps\common\Sentinels of the Multiverse\mods\BastionPublishing"
copy /y "C:\Users\%USERNAME%\source\repos\BastionPublishing\Resources\preview.png" "C:\Program Files (x86)\Steam\steamapps\common\Sentinels of the Multiverse\mods\BastionPublishing"
robocopy "C:\Users\%USERNAME%\source\repos\BastionPublishing\Resources\Atlas" "C:\Program Files (x86)\Steam\steamapps\common\Sentinels of the Multiverse\mods\BastionPublishing\Atlas" /e
robocopy "C:\Users\%USERNAME%\source\repos\BastionPublishing\Resources\Cutouts" "C:\Program Files (x86)\Steam\steamapps\common\Sentinels of the Multiverse\mods\BastionPublishing\Cutouts" /e
robocopy "C:\Users\%USERNAME%\source\repos\BastionPublishing\Resources\DeckBrowser" "C:\Program Files (x86)\Steam\steamapps\common\Sentinels of the Multiverse\mods\BastionPublishing\DeckBrowser" /e
robocopy "C:\Users\%USERNAME%\source\repos\BastionPublishing\Resources\Endings" "C:\Program Files (x86)\Steam\steamapps\common\Sentinels of the Multiverse\mods\BastionPublishing\Endings" /e
robocopy "C:\Users\%USERNAME%\source\repos\BastionPublishing\Resources\Environments" "C:\Program Files (x86)\Steam\steamapps\common\Sentinels of the Multiverse\mods\BastionPublishing\Environments" /e
robocopy "C:\Users\%USERNAME%\source\repos\BastionPublishing\Resources\Fonts" "C:\Program Files (x86)\Steam\steamapps\common\Sentinels of the Multiverse\mods\BastionPublishing\Fonts" /e
robocopy "C:\Users\%USERNAME%\source\repos\BastionPublishing\Resources\LargeCardTextures" "C:\Program Files (x86)\Steam\steamapps\common\Sentinels of the Multiverse\mods\BastionPublishing\LargeCardTextures" /e
robocopy "C:\Users\%USERNAME%\source\repos\BastionPublishing\Resources\Music" "C:\Program Files (x86)\Steam\steamapps\common\Sentinels of the Multiverse\mods\BastionPublishing\Music" /e
exit 0
