<img src="https://github.com/AlexVDV116/EliasLogAnalyzer/blob/main/src/Presentation/EliasLogAnalyzer.MAUI/Resources/Images/logo.png" alt="logo" width="300"/>

# ELIAS LogAnalyzer

ELIAS LogAnalyzer is een .NET MAUI-applicatie ontworpen om .log-bestanden te analyseren die zijn gegenereerd door de ELIAS-applicatie. Het helpt ontwikkelaars bij het debuggen en analyseren van bugs in de ELIAS-software, met als doel de tijd die besteed wordt aan handmatige buganalyse aanzienlijk te verminderen.

## Inhoudsopgave

1. [Belangrijkste Functies](#belangrijkste-functies)
2. [Technische Vereisten](#technische-vereisten)
3. [Installatie](#installatie)
4. [Aan de Slag](#aan-de-slag)
5. [Gebruik](#gebruik)
6. [Screenshots](#screenshots)
7. [Architectuur](#architectuur)
8. [Testen](#testen)
9. [Auteursrechten](#Auteursrechten)


## Belangrijkste Functies

- Laden en parsen van .log-bestanden naar LogFiles en LogEntries domeinmodellen.
- Getoonde gegevens in een verzameloverzicht met opties om logvermeldingen te sorteren en filteren.
- Marker en pin logvermeldingen voor gedetailleerde analyse.
- Analyseer logvermeldingen om hun waarschijnlijkheid van verwantschap met een bug te voorspellen.
- Visuele voorstellingen van logvermeldingen door middel van grafieken op een statistiekpagina.
- Integratie met TFS (Azure DevOps) voor geautomatiseerde bugrapportage.

## Technische Vereisten

- Multiplatform ondersteuning voor Windows en macOS.

## Installatie

Volg standaardprocedures voor de installatie van een .NET MAUI-applicatie. Zorg ervoor dat alle afhankelijkheden zijn vervuld voor uitvoering op zowel Windows als macOS-platformen.

## Aan de Slag

1. Open de applicatie.
2. Navigeer naar `File -> Open LogFiles` om een of meerdere .log-bestanden te selecteren.
3. De applicatie zal de logvermeldingen parsen en weergeven in een verzameloverzicht.

## Gebruik

- **Markeer Logvermeldingen**: Markeer een logvermelding als de hoofdvermelding voor analyse. De applicatie zal waarschijnlijkheden voorspellen gebaseerd op tijdverschillen, tickverschillen, en overeenkomsten in stacktrace/data-eigenschappen.
- **Pin Logvermeldingen**: Pin vermeldingen voor opname in rapporten.
- **Bekijken en Vergelijken**: Selecteer tot drie logvermeldingen om naast elkaar te tonen voor eenvoudige vergelijking.
- **Navigeer naar de Statistiekpagina**: Krijg een visuele voorstelling van de geladen logvermeldingen, waarbij een staafdiagram het type en aantal vermeldingen per bronbestand toont. Een taartdiagram geeft een weergave van het aantal verschillende logtypen, en een tijdlijn laat alle logvermeldingen zien op een tijdlijn, met de mogelijkheid om in en uit te zoomen op specifieke tijdvensters. Alle grafieken hebben een interactieve legenda waardoor de ontwikkelaar zich kan concentreren op specifieke logtypen.
- **Na de analyse**: Navigeer naar de rapportagepagina om een rapport in te vullen waarin de gemarkeerde en gepinde logvermeldingen worden opgenomen. Het rapport samen met de gemarkeerde en gepinde logvermeldingen zal worden ingediend bij de database voor latere referentie en zal automatisch een rapport indienen bij TFS (Azure DevOps bug tracking). De ontwikkelaar heeft ook de mogelijkheid om het rapport, inclusief logvermeldingen, in tekstformaat naar zijn klembord te kopiëren voor eenvoudig gebruik in andere toepassingen.

## Screenshots

![win_statisticspage](https://github.com/AlexVDV116/EliasLogAnalyzer/assets/98614832/f5e454cc-fe52-4b00-9121-555f10ac6714)
![win_reportpage](https://github.com/AlexVDV116/EliasLogAnalyzer/assets/98614832/92754c1e-3f00-43c0-a422-e512bfbd5031)
![win_logentriespage](https://github.com/AlexVDV116/EliasLogAnalyzer/assets/98614832/059342e4-5c9f-4d07-a82f-2781be3421b5)

## Architectuur

De applicatie is gebouwd met behulp van .NET MAUI voor de frontend en ASP.NET Core Web API voor de backend, met een SQL-database voor gegevensopslag. Dit zorgt voor een robuuste, schaalbare en makkelijk te onderhouden applicatiearchitectuur.

## Testen

Er zijn in totaal 67 tests geschreven, waarvan 9 integratietests en 58 unittests. Deze tests zijn gericht op het grondig testen van de logica in de services en de integratie van de viewmodels met de services.

## Auteursrechten

© 2024, Alex van der Velden. Alle rechten voorbehouden.

