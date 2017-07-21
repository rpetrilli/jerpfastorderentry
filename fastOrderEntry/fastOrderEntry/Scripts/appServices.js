var myModule = angular.module('appServices', [])

myModule.factory('listiniService', ['$http', function ($http) {
        var listiniService = {};

        var _getCategorie = function (url) {
            return $http.get(url).then(function (response) {
                return response;
            })
        }

        listiniService.getCategorie = _getCategorie;
        return listiniService;
}]);

myModule.factory('paginatoreService', ['$http', '$window', function ($http, win) {

    var factory = {};

    factory.next_record = function () {
        $scope.rec_pagina++;
        if ($scope.rec_pagina >= $scope.records.length ) {
            $scope.rec_pagina = 0;
        }
        $('#table tr:nth-child(' + ($scope.rec_pagina + 1) +') input.request-focus').focus();
    }

    factory.previous_record = function () {
        $scope.rec_pagina--;
        if ($scope.rec_pagina < 0) {
            $scope.rec_pagina = $scope.records.length - 1;
        }
        $('#table tr:nth-child(' + ($scope.rec_pagina + 1) + ') input.request-focus').focus();
    };    

    return factory;
}]);