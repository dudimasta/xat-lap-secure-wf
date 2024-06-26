# xat-lap-secure-wf
# Przykład Logic Apps z Visual Studio Code
Repo prezentuje użycie logic appek do pisania workflowów w VSC jako IDE. 
Repo bazuje na infrastrukturze założonej z projektu https://github.com/dudimasta/xat-lap-secure-infra

## Realizowany scenariusz
Na dużym podziomie ogólności:
- Az Funkcja <i>Functions\ProduceInvoice.cs</i> generuje ciąg znaków w XMLu.
- Workflow <i>\LogicApp\PushInvoice\workflow.json</i> wywołuje funkcję ProduceInvoice.cs i zapisuje zawartość w Az File w ścieżce <i>invoices/incoming/.xml</i>
- Drugi workflow odczytuje pliki z Az File, wykonuje ich transformację (wykorzystując mapy i reguły XSLT) 
i zapisuje je do Azure Event Grida

## Prezentowane koncepcje:
- programowanie LogicApps w VSC na lokalnej maszynie
    - Emulatory usług azurowych - Azurite (Blob Service, Table, Service, Queue Service)
    - uruchamianie i debuggowanie na lokalnej maszynie
- programowanie rozszerzeń w C# do workflowów uruchomionych w Logic Apps
- użycie map w Logic Apps
- użycie XSLT w Logic Apps
- użycie schematów w Logic Apps
- wywołanie deploymentu z VSC do Azura
- omówienie wyniku deploymentu
    - integracja z App Insights
    - zmienne środowiskowe
    - załadowane biblioteki, mapy, XSLT
- redeployment aplikacji z VSC


## Po kolei:
- w tym scenariuszu pracujesz z VSC, które jest uruchomione na Windowsie (nie w kontenerze)
- Aby budować funkcje lub nowe workflowy w tej bibliotece, należy otworzyć workspace [root git folder]/invdemo/invdemo.code-workspace
    - przełącz na workspace invdemo.code-workspace (File > Open workspace from file)
- zainstaluj rozszerzenia <b>Azure Logic Apps (Standard)</b>. Zrestartuj (ze dwa razy :-), po instalacji lub przełączeniu na workspace VSC dopiero przy restarcie sprawdza zależnośći i dociąga czego brakuje)
- w subskrypcji, do której zamierzasz robić deploymenty, włączyć provider 'Microsoft.OperationalInsights'
- wykonaj dotnet build projektu zawierającego kod rozszerzeń (Functions)
- wykonaj deployment z VSC (wskaż region, resource groupę, itd.)
    - Sometimes VS Code will want you to install a C# Dev Kit. If this has been installed, please remove it from the extensions. When C# Dev Kit is installed, it will add ".sln" files to your code project.  Delete these files as they can mess with the build tasks.
    - We have seen a couple situations where the APP_KIND isn't set. It should be set to workflowApp. You can find this setting in the Environment variables section. Set as follows:
    - <b>APP_KIND: workflowApp</b>
- Sprawdź, że w zmiennych środowiskowych zdeployowanej logi apki masz:
    - APP_KIND: workflowApp
    - AzureFile_connectionString: DefaultEndpointsProtocol=https;AccountName=[nazwa-az-file-storage];AccountKey=[TWÓJ-KLUCZ];EndpointSuffix=core.windows.net
- Spróbuj uruchomić workflow
    - powinien pojawić się błąd przy próbie zapisu pliku. 
    - Teraz należy wpuścić ruch z tej logic apki do Az File (utworzonego w skryptach dostępnych w https://github.com/dudimasta/xat-lap-secure-infra)

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
 