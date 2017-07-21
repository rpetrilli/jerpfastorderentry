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
