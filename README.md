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

## Po kolei:
- w tym scenariuszy pracujesz z VSC, które jest uruchomione na Windowsie (nie w kontenerze)
- zainstaluj rozszerzenia <b>Azure Logic Apps (Standard)</b>. Zrestartuj (ze dwa razy :-)) VSC
- upewnij się, że subskrypcja, do której będziesz wykonywać deploy funkcji w Resource Providerach ma włączone rozszerzenie 

### Workflowy w LogicApps
- Źródła:
    - https://learn.microsoft.com/en-us/azure/logic-apps/create-single-tenant-workflows-visual-studio-code
    - https://learn.microsoft.com/en-us/azure/logic-apps/create-maps-data-transformation-visual-studio-code
    - https://learn.microsoft.com/en-us/azure/logic-apps/add-run-csharp-scripts
    - https://learn.microsoft.com/en-us/azure/logic-apps/create-run-custom-code-functions
