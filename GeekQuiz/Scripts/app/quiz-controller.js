angular.module('QuizApp', [])
    .controller('QuizCtrl', function ($scope, $http) {
        $scope.answered = false;
        $scope.title = "loading question...";
        $scope.options = [];
        $scope.correctAnswer = false;
        $scope.working = false;

        $scope.answer = function () {
            return $scope.correctAnswer ? 'correct' : 'incorrect';
        };

        $scope.gettriviaquestions = function () {
            $scope.working = true;

            $http.get("/api/triviaquestion").success(function (data, status, headers, config) {
                $scope.triviaquestions = data;
                $scope.working = false;
            }).error(function (data, status, headers, config) {
                $scope.title = "Oops... something went wrong in gettriviaquestions";
                $scope.working = false;
            });
        };

        //Used to save a record after edit
        $scope.save = function (triviaquestion) {
            //alert("Edit");
            //alert(emp);
            var trivia = this.trivia;
            $http.put('/api/triviaquestion/' + triviaquestion.id, triviaquestion).success(function (data) {
                //alert("Saved Successfully!!");
                trivia.editMode = false;
            }).error(function (data) {
                $scope.title = "An Error has occured while Saving Trivia! " + data;
            });
        };

        //Used to add a new record ererere
        $scope.addtriviaquestion = function () {
            $scope.loading = true;
            $http.post('/api/triviaquestion/', this.newtriviaquestion).success(function (data) {
                //alert("Added Successfully!!");
                $scope.addMode = false;
                $scope.triviaquestions.push(data);
            }).error(function (data) {
                $scope.title = "An Error has occured while Adding Friend! " + data;

            });
        };

        //Used to delete a record
        $scope.deletetriviaquestion = function (triviaquestion) {
            var questiondid = triviaquestion.id;
            $http.delete('/api/triviaquestion/' + questiondid).success(function (data) {
                //alert("Deleted Successfully!!");
                $.each($scope.triviaquestions, function (i) {
                    if ($scope.triviaquestions[i].id === questiondid) {
                        $scope.triviaquestions.splice(i, 1);
                        return false;
                    }
                });
            }).error(function (data) {
                $scope.error = "An Error has occured while Saving TriviaQuestion! " + data;
            });
        };

        $scope.nextQuestion = function () {
            $scope.working = true;
            $scope.answered = false;
            $scope.title = "loading question...";
            $scope.options = [];

            $http.get("/api/triviaquestion/0").success(function (data, status, headers, config) {
                $scope.options = data.options;
                $scope.id = data.id;
                $scope.title = data.title;
                $scope.answered = false;
                $scope.working = false;
            }).error(function (data, status, headers, config) {
                $scope.title = "Oops... something went wrong in nextQuestion";
                $scope.working = false;
            });
        };

        $scope.sendAnswer = function (option) {
            $scope.working = true;
            $scope.answered = true;

            $http.post('/api/triviaanswers', { 'questionId': option.questionId, 'optionId': option.id }).success(function (data, status, headers, config) {
                $scope.correctAnswer = (data === true);
                $scope.working = false;
            }).error(function (data, status, headers, config) {
                $scope.title = "Oops... something went wrong in sendAnswer";
                $scope.working = false;
            });
        };

        $scope.toggleEdit = function () {
            this.trivia.editMode = !this.trivia.editMode;
        };
        $scope.toggleAdd = function () {
            $scope.addMode = !$scope.addMode;
        };

    });