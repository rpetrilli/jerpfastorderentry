var myModule = angular.module('webCore', []);

myModule.factory('tabController', function ($http) {
    var fac = {};

    fac.displayMode = 'list';
    fac.filtri = {};
    fac.current = null;
    fac.records = [];
    fac.url = '';
    fac.rec_x_pagina = 0;
    fac.rec_number = 0;
    fac.pag_number = 0;

    fac.setBaseUrl = function (url, show_message) {
        fac.url = url;
        fac.show_message = show_message;
    }


    fac.refresh_page = function (page_number) {
        fac.filtri.page_number = page_number;
        $http({
            method: 'GET',
            url: fac.url + '/getConenutoPagina',
            params: fac.filtri
        }).then(function (response) {
            fac.records = response.data;
            fac.pag_corrente = page_number;
            fac.rec_pagina = -1;
        }).catch(function (error, status) {
            fac.show_message('Errore', error.data);
        });
    }

    fac.refresh = function () {
        $http({
            method: 'GET',
            url: fac.url + '/getPaginatore',
            params: fac.filtri
        }).then(function successCallback(response) {
            fac.rec_x_pagina = response.data.rec_x_pagina;
            fac.rec_number = response.data.rec_number;
            fac.pag_number = response.data.pag_number;
            fac.refresh_page(0);
        }, function errorCallback(response) {
            fac.show_message('Errore', error.data);
        });
    }


    return fac;


});

