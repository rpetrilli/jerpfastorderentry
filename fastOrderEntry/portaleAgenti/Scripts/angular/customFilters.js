/// <reference path="../angular.js" />


angular.module("customFilters", [])
.filter("range", function () {
    return function (data, page, size) {
        if (angular.isArray(data) && angular.isNumber(page) && angular.isNumber(size)) {
            var start_index = (page - 1) * size;
            if (data.length < start_index) {
                return [];
            } else {
            	return data.slice(start_index, start_index + size ); 
            }
        } else {
            return data;
        }
    }
})
.filter('removeLeadingZeros', function() {
return function(tel) {
    if(typeof(tel) != undefined) {
        // Remove any leading zeros
        while(tel.charAt(0) === '0') {
            tel = tel.substr(1);
        }
        return tel.replace(" ", "");
    }
}
})
.filter("pageCount", function () {
    return function (data, size) {
        if (angular.isArray(data)) {
            var result = [];
            for (var i = 0; i < Math.ceil(data.length / size) ; i++) {
                result.push(i);
            }
            return result;
        } else {
            return data;
        } 
    }
});

