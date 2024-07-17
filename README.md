# xat-lap-secure-wf
# Przykład Logic Apps z Visual Studio Code
Repo prezentuje użycie logic appek do pisania workflowów w VSC jako IDE. 
Repo bazuje na infrastrukturze założonej z projektu https://github.com/dudimasta/xat-lap-secure-infra

## Realizowany scenariusz
Na dużym podziomie ogólności:
- Az Funkcja <i>Functions\ProduceInvoice.cs</i> generuje ciąg znaków w XMLu, który udaje fakturę. W scenariuszu przetwarzamy dwa rodzaje faktur - krajowe i zagraniczne.
- Workflow <i>\LogicApp<u>\PushInvoice</u>\workflow.json</i> wywołuje funkcję ProduceInvoice.cs i zapisuje zawartość w Az File w ścieżce <i>[Az File share]/invoices/incoming/.xml</i>
- Drugi workflow - <i>\LogicApp<u>\Invoice_ETL_ToGrid</u>\workflow.json</i> odczytuje pliki z Az File, wykonuje ich transformację (wykorzystując mapy i reguły XSLT) 
i zapisuje je do Azure Event Grida
    - po zaczytaniu pliku z Az File wykonuje walidację komunikatu względem ustalonego kontraktu, czyli pliku zapisanego w LogicApp\Artifacts\Schemas<b>\sampleInvoice.xsd</b>
    - jeśli OK, to wykonuje parsowanie komunikatu przychodzącego - określa, czy faktura jest krajowa czy zagraniczna
    - dla krajowych:
        - transformuje do JSON zgodnie ze XSLT z pliku /LogicApp/Arifacts/Maps/sample_xsl.xml
        - zapisuje <b>komunikat JSON</b> do event grida dedykowanego dla faktur krajowych
    - dla zagranicznych
        - transformuje fakturę do formatu pliku płaskiego CSV zgodnie z instrukcjami transformacji z pliku XSLT: <i>Artifacts\Maps\InvoiceToCSV2.xml</i>
        - zapisuje komunikat tekstowy do kolejki ASBQ dedykowanej dla faktur zagranicznych

## Prezentowane koncepcje:
- programowanie LogicApps w VSC na lokalnej maszynie
    - Emulatory usług azurowych - Azurite (Blob Service, Table, Service, Queue Service)
    - uruchamianie i debuggowanie na lokalnej maszynie
- programowanie rozszerzeń w C# do workflowów uruchomionych w Logic Apps
    - workfow \invdemo\LogicApp\PushInvoice\workflow.json wykorzystuje niestandardową logikę napisaną w C# w \invdemo\Function\ProduceInvoice.cs
- użycie XSL w Logic Apps
    - /LogicApp/Arifacts/Maps/sample_xsl.xml - to mapa, która jest wykorzystana do przetłumaczenia zaczynanego pliku XML z fakturą do postaci JSON.
- użycie schematów XSD (dla XMLi) w Logic Apps - np. aktywność "XML Validation"
- wywołanie deploymentu z VSC do Azura
- omówienie wyniku deploymentu
    - integracja z App Insights
    - zmienne środowiskowe
    - załadowane biblioteki, mapy, XSLT
- redeployment aplikacji z VSC
- pętle w Logic Apps domyślnie chodzą równolegle, tzn. jeśli korzystają ze wspólnych zmiennych globalnych, to nawzajem je sobie nadpisują. Aby synchronizować dostęp do zmiennych globalnych należy ustawić <b>DOP na 1</b>


## Prereqs:
- Extensions: 
    - Azure Account
    - Azure Logic Apps (Standard)
- VSC zaproponuje instację rozszerzenia C# Dev Kit. W tym przypadku <b>nie należy instalować tego rozszerzenia</b>
- wyedytuj plik invdemo.code-workspace i zmień ścieżki w "settings", mają wskazywać na lokalizację zalogowanego usera

## Po kolei:
### Zapoznanie się z zawartością repo
- w tym scenariuszu pracujesz z VSC, które jest uruchomione na Windowsie (nie w kontenerze)
- Aby budować funkcje lub nowe workflowy w tej bibliotece, należy otworzyć workspace [root git folder]/invdemo/invdemo.code-workspace
    - przełącz na workspace invdemo.code-workspace (File > Open workspace from file)
- zainstaluj rozszerzenia <b>Azure Logic Apps (Standard)</b>. Zrestartuj (ze dwa razy :-), po instalacji lub przełączeniu na workspace VSC dopiero przy restarcie sprawdza zależnośći i dociąga czego brakuje)
- przejrzyj kod w plikach:
    - ProduceInvoice.cs
    - objects.cs
- wykonaj <code>dotnet build .\ProduceInvoice.csproj</code> projektu zawierającego kod rozszerzeń (Functions). W wyniku buildu biblioteki zostaną wrzucone do [...]\LogicApp\lib\custom\net472
- Przejrzyj w designerze workflow PushInvoice
- uzupełnij plik local.settings.json na bazie przykładu sample.local.settings.json
    - w przypadku produkcyjnym klucz należałoby pobrać z właściwego (Prod, UAT, Test, Perf itd) key vaulta

### Uruchom workflow lokalnie
- wyedytuj workflow PushInvoice w Designerze, w akcji Create File ustaw nazwę Az File, właściwość Folder Path, np "rdu-lab-file-share"
    - w przypadku produkcyjnym, nazwy zasobów powinny być pobierane ze zmiennych a nie hardkodowane
- w Az File Storage załóż ścieżkę: invoices/incoming/
    - w przypadku produkcyjnym taki element infry powinien być stworzony ze skryptów przygotowujących infrę
- uruchom emulatory Azurite, podaj LogicApp jako folder, gdzie będą przechowywać dane
- ustaw breakpoint w pliku ProduceInvoice.cs w metodzie run
- uruchom workflow: Debug > Attach to Logic App
- dołącz do debuggera runtime dla Az Functions: Debug > Attach to Function
- Uruchom workflow overview > run trigger
- sprawdź, że w az File stworzyła się faktura

### Deployment do Azure
- w subskrypcji, do której zamierzasz robić deploymenty, włączyć provider 'Microsoft.OperationalInsights'
    - sub > settings > resource providers > Microsoft.OperationalInsights > register
- wykonaj deployment z VSC (wskaż region, resource groupę, itd.)
    - InvDemo workspace > LogicApp > rmb > deploy to logicApp > new LogicApp (Advanced)
    - Sometimes VS Code will want you to install a C# Dev Kit. If this has been installed, please remove it from the extensions. When C# Dev Kit is installed, it will add ".sln" files to your code project.  Delete these files as they can mess with the build tasks.
    - We have seen a couple situations where the APP_KIND isn't set. It should be set to workflowApp. You can find this setting in the Environment variables section. Set as follows:
    - <b>APP_KIND: workflowApp</b>
- Sprawdź, że w zmiennych środowiskowych zdeployowanej logic apki masz:
    - APP_KIND: workflowApp
    - AzureFile_connectionString: DefaultEndpointsProtocol=https;AccountName=[nazwa-az-file-storage];AccountKey=[TWÓJ-KLUCZ];EndpointSuffix=core.windows.net
- W storage Az File załóż strukturę folderów:
    - /invoices/incoming
- Spróbuj uruchomić workflow
    - powinien pojawić się błąd przy próbie zapisu pliku - W "logu" workflowu krok "Create file" powinien mieć błąd "Forbidden"
    - Teraz należy wpuścić ruch z tej logic apki do Az File (utworzonego w skryptach dostępnych w https://github.com/dudimasta/xat-lap-secure-infra). Skrypty w Infra tworzą i konfigurują dostęp do Az File z VNetu, który jest stworzony w RG zawierającej LogicApp. Po deploymencie logic apki do Azure, podłącz ją pod VNet w tym celu:
    - otwórz Logic Apkę w Azure portal > Networking > Outgoing > Vnet integration, wybierz vnet i subnet utworzony przez skrypty z infry. Uruchom WF ponownie i sprawdź, że w Az File masz nowy plik

- Podobnie przejrzyj i zrób deployment workflowu Invoice_ETL_ToGrid
    - ustaw connection strings, access keys etc
    - załóż katalogi w Az File:
        - invoices/processing/
        - invoices/processedError/
        - invoices/processedOK/

### Demo: Utworzenie nowego workflowu
    
- !!! po skończeniu testów usuń komponenty z azura !!!

## Pamiętaj o kosztach
W momencie pisania tego readme, MS powoli wycofuje consumption plan LogicApps na rzecz Standard Logic Apps. Zresztą tylko w standard LogicApps są zaimplementowane np. wsparcie dla VNetów, private endpointów, czy integracja z App Insights.
Problem polega na tym, że Standard Logic Apps są płane up-front i to sporo. Więc, żeby nie generować niepotrzebych kosztów na licencji deweloperskiej:
- testuj i programuj głównie w VSC
- jedynie ostatnie testy rób po deploymencie do Azura
- po skończeniu testów usuń komponenty z azura


### Workflowy w LogicApps
- Źródła:
    - https://learn.microsoft.com/en-us/azure/logic-apps/create-single-tenant-workflows-visual-studio-code
    - https://learn.microsoft.com/en-us/azure/logic-apps/create-maps-data-transformation-visual-studio-code
    - https://learn.microsoft.com/en-us/azure/logic-apps/add-run-csharp-scripts
    - https://learn.microsoft.com/en-us/azure/logic-apps/create-run-custom-code-functions
    - custom C# code (net 472): https://techcommunity.microsoft.com/t5/azure-integration-services-blog/announcement-introducing-net-framework-custom-code-for-azure/ba-p/3847711
    - custom C# code (net core 8):https://techcommunity.microsoft.com/t5/azure-integration-services-blog/announcement-introducing-net-8-custom-code-support-for-azure/ba-p/4167893
 

 ## Branching tips:
 Open your repository in Visual Studio Code: You can do this by opening Visual Studio Code, clicking on File > Open Folder and then navigating to your repository’s location on your local machine.
Open the integrated terminal: You can open the terminal in Visual Studio Code by clicking on View > Terminal or using the shortcut Ctrl + `.
Fetch all branches from the remote repository: Before creating a new branch, it’s a good practice to fetch all the branches from the remote repository. You can do this by running the following command in the terminal:
<br /><code>git fetch origin</code>

Check out to the main branch: You can switch to the main branch by running the following command in the terminal:
<br /><code>git checkout main</code>

Pull the latest changes from the main branch: You can pull the latest changes from the main branch by running the following command in the terminal:
<br /><code>git pull origin main</code>

Create and switch to the new branch: You can create a new branch called myDevBranch and switch to it by running the following command in the terminal:
<br /><code>git checkout -b myDevBranch</code>

Push the new branch to the remote repository: Finally, you can push the new branch to the remote repository by running the following command in the terminal:
<br /><code>git push -u origin myDevBranch</code>

You can check which branch you have currently checked out in Git by using the following command in your terminal:
<br /><code>git branch --show-current</code>