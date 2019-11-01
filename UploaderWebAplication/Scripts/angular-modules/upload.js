var app = angular.module("uploadApp", ['ngFileUpload']);
app.run(function () { });
   /* 
app.controller("uploadController", ['$scope', 'Upload', '$timeout', function ($scope, Upload, $timeout)
    {
        $scope.message="ggg"
        $scope.onFileSelect = function ($files) {
            Upload.upload({
                url: 'api/upload',
                file: $files,
            }).progress(function (e) {
            }).then(function (data, status, headers, config) {
                // file is uploaded successfully
                console.log(data);
            }); 
        };
    }]);*/
app.controller('uploadController', ['$scope', 'Upload', '$timeout', function ($scope, Upload, $timeout) {
    $scope.uploadFiles = function (files, errFiles) {
        $scope.files = files;
        $scope.errFiles = errFiles;
        $scope.msg=''
        angular.forEach(files, function (file) {
            file.upload = Upload.upload({
                url: 'api/upload',
                data: { file: file }
            });
           
            file.upload.then(function (response) {
                $timeout(function () {
                    file.result = response.data;
                    $scope.msg = response.statusText;
                });
            }, function (response) {
                    if (response.status > 0) {                                              
                        $scope.msg = response.data.Message;
                    }
            }, function (evt) {
                file.progress = Math.min(100, parseInt(100.0 *
                    evt.loaded / evt.total));
            });
        });
    }
}]);

app.controller('tranController', ['$scope', '$http', function ($scope, $http) {
    $http({
        method: 'GET',
        url: '/api/currency/get'
    }).then(function (response) {
        $scope.currencies = response.data;
      
    }, function errorCallback(response) {
       
    });
    $scope.getbycurrency = function () {
        id = $scope.currency.Id
        $http({
            method: 'POST',
            url: '/api/gettran/bycurrency',
            data: id
        }).then(function (response) {
            $scope.invoices = response.data;
            
        }, function errorCallback(response) {

        });
    };
    $scope.getbystatus = function () {     
        $http({
            method: 'POST',
            url: '/api/gettran/bystatus',
            data: $scope.status
        }).then(function (response) {
            $scope.invoices = response.data;

        }, function errorCallback(response) {

        });
    };
    $scope.getbydate = function () {
      
        $http({
            method: 'POST',
            url: '/api/gettran/bydate',
            data: JSON.stringify({'from':'2019-01-01', 'till':'2019-10-01'})
        }).then(function (response) {
            $scope.invoices = response.data;
        }, function errorCallback(response) {

        });
    };
}]);