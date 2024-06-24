# xat-lap-secure-wf
# Przykład Logic Apps z Visual Studio Code
Repo prezentuje użycie logic appek do pisania workflowów w VSC jako IDE. Repo używa koncepcji devcontainers
jako sposobu na izolację środowiska od innych komponentów pracujących na hoście.
Do uruchomienia sugerowane jest posiadanie zainstalowanego środowiska kontereryzacji - Dockera. 

Repo składa się z dwóch workspaces:
- infra - zawiera skrypty (w bicep i az cli), które tworzą i konfigurują komponenety stacku azurowego
- 

## Realizowany scenariusz
Na dużym podziomie ogólności:
- Jeden workflow generuje pliki w formacie .xml i zapisuje je w Az File
- Drugi workflow odczytuje pliki z Az File, wykonuje ich transformację (wykorzystując mapy i reguły XSLT) 
i zapisuje je do Azure Event Grida

## Prezentowane koncepcje:
- zastosowanie skryptów automatyzujących tworzenie wymaganych komponentów w Azure - Bicep
- bezpieczna komunikacja przy użyciu prywatnych VNETów - temat często podkreślany przez klientów
    - Tworzenie Vnet, dzielenie na podsieci, tworzenie prywantnych endpointów we wskazanych podsieciach
    - ograniczenie dostępu do komunikacji z vnetów i prywatnych endpointów
- programowanie rozszerzeń w C# do workflowów uruchomionych w Logic Apps
- użycie map w Logic Apps
- użycie XSLT w Logic Apps
- użycie schematów w Logic Apps

## Żeby nie było za łatwo - topologia:
- Używane są cztery resource grupy - idealnie jeśli w testach każda będzie w innym regionie azurowym
- listę funkcjonalności dostępnych w regionach można znaleźć tu: https://azure.microsoft.com/en-us/explore/global-infrastructure/geographies 

# Aby odpalić:
## Zainstalować prerequisities:
- https://learn.microsoft.com/en-us/azure/logic-apps/create-single-tenant-workflows-visual-studio-code 
- az cli
    - linux: 
        <code>curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash</code>
    - <code>az login --use-device-code</code>
    - <code>az login --use-device-code</code>
    - list of az datacenters: 
        <code>az account list-locations -o table</code>

## Inne
- utworzyć plik o nazwie ./infra/.env - można użyć jako template'u pliku przykładowego: ./infra/sample.env

## Co się dzieje po kolei
### Infrastruktura i zabezpieczenia
- Utworzenie komponentów podstawowywch zgodnie instrukcjami w main.bicep. Aby uruchomić: 
</br><code>cd ./infra/</br>sh ./deploy_base.sh</code>. W wyniku uruchomienia utworzą się
    - Resource Groups
    - vnet i z podziałem podział vnetu na subnety (automatyzacja w modułach
</br><code>v-net.bicep, subnet.bicep</code>, wywoływanych z main.bicep)
    - blob storage, a w nim zasobu az files
    </br><code>az-files.bicep</code>
    - Maszyna wirtualna z Linuxem - w celu sprawdzenia czy reguły ograniczenia dostępu do Az File działają w warstwie sieciowej (v-nets, private endpoints). Po deploymencie sprawdzić, ze z Linuxa jest dostęp do Az Files. Aby sprawdzić dostęp do Az File z VM:
        - zaloguj się do shella Linuxa, 
            - za wpuszczenie ruchu odpowiadają reguły firewalla zdefiniowane w NSG o nawie danej zmienną środowiskową LINUX_VM_NSG_NAME. Aby zobaczyć namiary na maszynę z linuxem, uruchom:
                - <code>sh ./linux-vm-show-conn-details.sh</code>
                - będziesz potrzebować jakiegoś edytora. Przy pierwszym logowaniu wykonaj następujące polecenia:
                    - <code>sudo apt update && sudo apt upgrade -y</code>
                    - <code>sudo apt install vim</code>
        - podmontuj zasób z Az File do lokalnego systemu plików. Skrypt to zamontowania zasobu Az File na Windows/ Linux/ MacOS można pobrać z Portal Azure. Wybierz file share > Connect > rodzaj OSa. Skopiuj skrypt i uruchom na maszynie testowej
            - będziemy testować nadawanie/usuwanie dostępów, skrypt skopiowany z Azure Portal możesz zapisaćw pliku np. w lokalizacji usera: np. <code>vim mount.sh</code> i potem <code>sh ./mount.sh</code>Wykonanie tego skryptu spowoduje, że zasób z AzFile będzie dostępny także po restarcie, bez konieczności ponownego uruchomienia. (Aby wejść w tryb edycji w VIM, po uruchomieniu programu naciśnij a. Gdy skończysz edycję pliku, naciścij kombinację: Esc+:+w+q - czyli Write and Quit. Więcej o edytowaniu w VIM np tu: https://vim.rtorr.com/)
        - załóż plik, wyedytuj, zapisz
        - sprawdź, że pliki są widoczne z portalu Az w inspekcji zawartości udostępninych zasobów
- Po tym jak udało się skomunikować z Az File z komputera pracującego w innym regionie geograficznym, zabezpieczmy ruch. Celem poniższych kroków jest aby Az File akceptował komunikację z jawnie wskazanych podsieci należących do sieci prywatnej (private vnet). W prezentowanym przykładzie komunikacja zostanie zabezpieczona z użyciem reguł ustaionych, tzn. będzie szła po prywantym VNet - czyli service endpoints (https://learn.microsoft.com/en-us/azure/virtual-network/virtual-network-service-endpoints-overview)
    - Rozpoczynamy od zamknięcia całego ruchu do Az File, który przychodzi po internecie publicznym. Poniższy skrypt uruchamia polecenia Az Cli, które wyłączają dostęp domyślny: 
    </br><code>sh ./az-file-deny-access.sh</code>
    </br>Po uruchomieniu tego skryptu, możesz sprawdzić, że dostęp do Az File z testowej maszyny linuxowej został wyłączony. Zawartość nie jest dostępna.
    - Po zweryfikowania, że domyślnie dostęp został wyłączony, konfigurujemy ACL (access control list), w której wskazujemy że ma zostać wpuszczony ruch z subnetu, z którym jest skojarzony NIC (Network Interface Card) maszyny wirtualnej z linuxem. </br>
    <code>sh ./az-file-acl-add-linux-subnet.sh</code></br>
    W wyniku uruchomienia skryptu maszyna z linuxem (a właściwie subnet, do którego jest podpięta) uzyskuje dostęp do zasobów Az File.

### Workflowy w LogicApps
- Źródła:
    - https://learn.microsoft.com/en-us/azure/logic-apps/create-single-tenant-workflows-visual-studio-code
    - https://learn.microsoft.com/en-us/azure/logic-apps/create-maps-data-transformation-visual-studio-code
    - https://learn.microsoft.com/en-us/azure/logic-apps/add-run-csharp-scripts
    - https://learn.microsoft.com/en-us/azure/logic-apps/create-run-custom-code-functions
    