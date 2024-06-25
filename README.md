# xat-lap-secure-wf
# Przykład Logic Apps z Visual Studio Code
Repo prezentuje użycie logic appek do pisania workflowów w VSC jako IDE. 

## Realizowany scenariusz
Na dużym podziomie ogólności:
- Az Funkcja <i>Functions\ProduceInvoice.cs</i> generuje ciąg znaków w XMLu.
- Workflow <i>\LogicApp\PushInvoice\workflow.json</i> wywołuje funkcję ProduceInvoice.cs i zapisuje zawartość w Az File w ścieżce <i>invoices/incoming/.xml</i>
- Drugi workflow odczytuje pliki z Az File, wykonuje ich transformację (wykorzystując mapy i reguły XSLT) 
i zapisuje je do Azure Event Grida

## Prezentowane koncepcje:
- programowanie rozszerzeń w C# do workflowów uruchomionych w Logic Apps
- użycie map w Logic Apps
- użycie XSLT w Logic Apps
- użycie schematów w Logic Apps
- Aby budować funkcje lub nowe workflowy w tej bibliotece, należy otworzyć workspace [root git folder]/invdemo/invdemo.code-workspace

## Po kolei:
- w tym scenariuszy pracujesz z VSC, które jest uruchomione na Windowsie (nie w kontenerze)
- zainstaluj rozszerzenia <b>Azure Logic Apps (Standard)</b>. Zrestartuj (ze dwa razy :-)) VSC
- w subskrypcji włączyć provider 'Microsoft.OperationalInsights'
- Sometimes VS Code will want you to install a C# Dev Kit. If this has been installed, please remove it from the extensions. When C# Dev Kit is installed, it will add ".sln" files to your code project.  Delete these files as they can mess with the build tasks.
- We have seen a couple situations where the APP_KIND isn't set. It should be set to workflowApp. You can find this setting in the Environment variables section. Set as follows:
    - APP_KIND: workflowApp


### Workflowy w LogicApps
- Źródła:
    - https://learn.microsoft.com/en-us/azure/logic-apps/create-single-tenant-workflows-visual-studio-code
    - https://learn.microsoft.com/en-us/azure/logic-apps/create-maps-data-transformation-visual-studio-code
    - https://learn.microsoft.com/en-us/azure/logic-apps/add-run-csharp-scripts
    - https://learn.microsoft.com/en-us/azure/logic-apps/create-run-custom-code-functions
    - custom C# code (net 472): https://techcommunity.microsoft.com/t5/azure-integration-services-blog/announcement-introducing-net-framework-custom-code-for-azure/ba-p/3847711
