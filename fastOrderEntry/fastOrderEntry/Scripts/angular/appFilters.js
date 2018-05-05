var app = angular.module('appFilters', [])

app.filter('imponibile', function () {
    return function (value) {
        var qta = (value.qta_in_consegna > 0) ? value.qta_in_consegna : value.qta_ordinata;        
        return qta * value.prezzo_vendita * (100 - value.sconto_1) / 100 * (100 - value.sconto_2) / 100 * (100 - value.sconto_3) / 100 * (100 - value.sconto_agente) / 100;
    }
});

app.filter('prezzo', function () {
    return function (value) {
        return value.prezzo_vendita * (100 - value.sconto_1) / 100 * (100 - value.sconto_2) / 100 * (100 - value.sconto_3) / 100 * (100 - value.sconto_agente) / 100;
    }
});

app.filter('prezzoacq', function () {
    return function (value) {
        return value.prezzo_acquisto * (100 - value.sconto_a_1) / 100 * (100 - value.sconto_a_2) / 100 * (100 - value.sconto_a_3) / 100;
    }
});

app.filter('iva', function ($filter) {
    return function (value) {
        var imponibile = $filter('imponibile')(value);
        return imponibile * value.aliquota / 100;
    }
});

app.filter('margine', function ($filter) {
    return function (value) {
        return ($filter('prezzo')(value) - $filter('prezzoacq')(value)) / $filter('prezzo')(value) * 100;
    }
});

app.filter('margineordine', function ($filter) {
    return function (value) {
        return ($filter('prezzo')(value) - value.prezzo_acquisto) / $filter('prezzo')(value) * 100;
    }
});

app.filter('peso', function () {
    return function (value) {
        return parseFloat(value.peso_netto) / 1000;
    }
});

app.filter('totpz', function ($filter) {
    return function (ordine) {
        var tot = 0;
        if (!ordine.righe) {
            return 0;
        }
        for (var i = 0; i < ordine.righe.length; i++) {
            tot += ordine.righe[i].qta_ordinata;
        }
        return tot;
    }
});


app.filter('totpeso', function ($filter) {
    return function (ordine) {
        var tot = 0;
        if (!ordine.righe) {
            return 0;
        }
        for (var i = 0; i < ordine.righe.length; i++) {
            tot += ((ordine.righe[i].peso_netto / 1000) * ordine.righe[i].qta_ordinata);
        }
        return tot;
    }
});

app.filter('totimponibile', function ($filter) {
    return function (ordine) {
        if (!ordine.righe) {
            return 0;
        }
        var tot = 0;
        for (var i = 0; i < ordine.righe.length; i++) {
            tot += $filter('imponibile')(ordine.righe[i]);
        }
        return tot;
    }
});

app.filter('totiva', function ($filter) {
    return function (ordine) {
        if (!ordine.righe) {
            return 0;
        }
        var tot = 0;
        for (var i = 0; i < ordine.righe.length; i++) {
            tot += $filter('iva')(ordine.righe[i]);
        }
        return tot;
    }
});