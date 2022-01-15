# NBPProjekat1_2


Napomena: Koristio sam ASP.NET Core 6, koji koliko ja znam, mora postoji samo na Visual Studio 2022.
Postoje funkcije u Logic klasama koje sam napisao ali nisam implementirao u projektu, neke zbog nedostatka vremena, ali vecina zbog toga sto nema smisla te funkcije koristiti.
Za sub/pub koristim SignalR koji kad backbone koristi Redis. Ovo je jedino resenje koje sam ja pronasao koje bih imao vremena da proucim i implementiram na vreme.
Pub se na strani administracije salje dok se pub moze primiti samo na strani info za klub. Najverovatnije postoji nacin da se napravi da radi site wide, ja sam citao sa ms sajta,
i taj projekat je konfigurisan za one page signalR. U redisLogic postoje f-je za redis sub/pub koje sam napisao ali program mi je pucao kad sam probao da ih pozovem.
