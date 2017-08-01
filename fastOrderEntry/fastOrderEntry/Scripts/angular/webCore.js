var myModule = angular.module('webCore', []);

myModule.factory('tabController', function ($http) {
    var factory = {};

    factory.createController = function (prop) {
        var fac = {};
        fac.displayMode = 'list';
        fac.filtri = {};
        fac.current = null;
        fac.records = [];

        fac.rec_x_pagina = 0;
        fac.rec_number = 0;
        fac.pag_number = 0;

        fac.nr_pagine_display = 4;


        fac.prop = prop;
        


        fac.refresh_page = function (page_number) {
            fac.filtri.page_number = page_number;
            $http({
                method: 'GET',
                url: fac.prop.api_root + '/getConenutoPagina',
                params: fac.filtri
            }).then(function (response) {
                fac.records = response.data;
                fac.pag_corrente = page_number;
                fac.rec_pagina = -1;
            }).catch(function (error, status) {
                fac.prop.message_function('Errore', error.data);
            });
        };

        fac.refresh = function () {
            $http({
                method: 'GET',
                url: fac.prop.api_root + '/getPaginatore',
                params: fac.filtri
            }).then(function successCallback(response) {
                fac.rec_x_pagina = response.data.rec_x_pagina;
                fac.rec_number = response.data.rec_number;
                fac.pag_number = response.data.pag_number;
                fac.refresh_page(0);
            }, function errorCallback(response) {
                fac.prop.message_function('Errore', error.data);
            });
        };


        fac.next_record = function () {
            fac.rec_pagina++;
            if (fac.rec_pagina >= fac.records.length) {
                fac.rec_pagina = 0;
            }
            fac.prop.select_row_call_back(fac.rec_pagina + 1);
        };
        fac.previous_record = function () {
            fac.rec_pagina--;
            if (fac.rec_pagina < 0) {
                fac.rec_pagina = fac.records.length - 1;
            }
            fac.prop.select_row_call_back(fac.rec_pagina + 1);
        };

        fac.nr_pagina_paginatore = function (indice) {
            if (indice + 1 > fac.pag_number) {
                return -1;
            } else if (fac.pag_corrente + 1 < fac.nr_pagine_display) {
                return indice;
            } else {
                return fac.pag_corrente + indice - fac.nr_pagine_display + 1;
            }
        };


        fac.deleteItem = function (item) {
            bootbox.confirm("Intendi procedere?", function (result) {
                if (result) {
                    $http({
                        method: 'DELETE',
                        url: fac.prop.api_root + '/delete',
                        data: fac.current
                    }).then(function (response) {
                        fac.displayMode = "list";
                        fac.refresh_page(fac.filtri.page_number);
                    }).catch(function (error, status) {
                        fac.prop.message_function('Errore', error.data);
                    });
                }
            });
        };


        fac.edit = function (item) {
            fac.nuovo = false;

            $http({
                method: 'GET',
                url: fac.prop.api_root + '/select',
                params: item
            }).then(function (response) {
                fac.displayMode = "edit";
                fac.current = response.data;
            }).catch(function (error, status) {
                fac.prop.message_function('Errore', error.data);
            });

        };


        fac.create = function (item) {
            fac.nuovo = true;

            fac.displayMode = "edit";
            fac.current = {};

        };

        fac.saveEdit = function (item) {
            if (fac.nuovo) {
                fac.insert(item);
            } else {
                fac.update(item);
            }
        };


        fac.insert = function (item) {
            $http({
                method: 'POST',
                url: fac.prop.api_root + '/insert',
                data: fac.current
            }).then(function (response) {
                fac.displayMode = "list";
                fac.refresh_page(fac.filtri.page_number);
            }).catch(function (error, status) {
                fac.prop.message_function('Errore', error.data);
            });
        };

        fac.update = function (item) {
            $http({
                method: 'PUT',
                url: fac.prop.api_root + '/update',
                data: fac.current
            }).then(function (response) {
                fac.displayMode = "list";
                fac.refresh_page(fac.filtri.page_number);
            }).catch(function (error, status) {
                fac.prop.message_function('Errore', error.data);
            });
        };


        fac.cancelEdit = function () {
            fac.current = {};
            fac.displayMode = "list";
            fac.refresh_page(fac.filtri.page_number);
        };

        return fac;
    }


    return factory;

});

myModule.directive('tabControllerPag', function () {
    return {
        restrict: 'AE',
        replace: 'true',
        templateUrl: '/Scripts/angular/webCoreTabControllerPag.tpl.html',
        scope: {
            tc: '=tableController'
        },
        link: function (scope, elem, attrs) {           
            //scope[intTableController] = scope[attrs['tableController']];
        }
    };
});

myModule.filter('trimZeros', function () {
    return function (x) {
        return x.replace(/^0+/, '');
    };
});



myModule.directive('typehead', function () {
    return {
        scope: {
            onSelectEvent: '=onSelect',
            addParams: '=params'
        },
        link: function (scope, element, attrs, modelCtrl) {
            element.typeahead({
                source: function (query, process) {
                    var rss = [];
                    mapObj = {};

                    var params = { query: query };
                    if (typeof (scope.addParams) != 'undefined') {
                        Object.assign(params, scope.addParams);
                    }

                    return $.post(attrs['typehead'], params , function (data) {
                        $.each(data, function (i, rs) {
         
                            mapObj[rs.name] = rs;
                            rss.push(rs.name);
                        });
                        process(rss);
                    });
                },
                updater: function (item) {
                    var obj = mapObj[item];
                    scope.$apply(function () {
                        scope.onSelectEvent(obj);
                        //eval('scope.'+attrs['filterRef'] + '=\'' + id + '\'');
                        //scope.tc.refresh(scope.pag_corrente);
                    });
                    return item;
                }
            }).on("keyup", function () {
                if (!$(this).val()) {
                    scope.$apply(function () {
                        scope.onSelectEvent();
                        //eval('scope.' + attrs['filterRef'] + '=\'\'');
                        //scope.tc.refresh(scope.pag_corrente);
                    });
                }
            });

        }
    };
});
