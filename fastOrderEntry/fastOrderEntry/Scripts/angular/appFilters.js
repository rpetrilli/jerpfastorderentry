var app = angular.module('appFilters', [])

app.filter('imponibile', function () {
    return function (value) {
        var qta = (value.qta_in_consegna > 0) ? value.qta_in_consegna : value.qta_ordinata;
        return qta * value.prezzo_vendita * (100 - value.sconto_1) / 100 * (100 - value.sconto_2) / 100 * (100 - value.sconto_3) / 100;
    }
});

app.filter('iva', function ($filter) {
    return function (value) {
        var imponibile = $filter('imponibile')(value);
        return imponibile * value.aliquota / 100;
    }
});

app.filter('totpeso', function ($filter) {
    return function (ordine) {
        var tot = 0;
        for (var i = 0; i < ordine.righe.length; i++) {
            tot += ordine.righe[i].peso_lordo;
        }
        return tot;
    }
});

app.filter('totimponibile', function ($filter) {
    return function (ordine) {
        var tot = 0;
        for (var i = 0; i < ordine.righe.length; i++) {
            tot += $filter('imponibile')(ordine.righe[i]);
        }
        return tot;
    }
});

app.filter('totiva', function ($filter) {
    return function (ordine) {
        var tot = 0;
        for (var i = 0; i < ordine.righe.length; i++) {
            tot += $filter('iva')(ordine.righe[i]);
        }
        return tot;
    }
});