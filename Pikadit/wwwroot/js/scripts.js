const uri = "/api/posts/";
const uriusers = "/api/users/";
const uricoms = "/api/comments/";

$(document).ready(function (event) {
    AccountManager.Login(0);//авторизация пользователя
    $('#addPostBut').click(function (e) {
        e.preventDefault();
        PostManager.createPost();
    });
    $('#newPost').click(function (e) {
        e.preventDefault();
        setaddvisible();
    });
    $('#Registerbtn').click(function (e) {
        e.preventDefault();
        AccountManager.Register();
    });
    $('#Loginbtn').click(function (e) {
        e.preventDefault();
        AccountManager.Login(1);
    });
    $('#Logoffbtn').click(function (e) {
        e.preventDefault();
        AccountManager.Logoff();
    });
    $('#editPostBut').click(function (e) {
        e.preventDefault();
        PostManager.editPost(postid);
    });
    $('#newCommentBut').click(function (e) {
        e.preventDefault();
        CommentManager.AddComment();
    });
});

var AccountManager = function () {
    function Register() {
        var user = {//считывание информации пользователя
            userName: $("#username").val(),
            email: $("#email").val(),
            password: $("#password").val()
        };
        let request = new XMLHttpRequest();//построение запроса
        request.open("POST", "api/Register");
        request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        request.onload = function () {
            if (request.status === 200) {//если пользователь успешно добавлен
                var checkuser = JSON.parse(request.responseText);
                noteToHTML("Вы зарегестрированы");
                SessionManager.ToSession(checkuser);
                GetKarma();
                $("#username").val("");
                $("#email").val("");
                $("#password").val("");
                $(".close").click();
            }
            else if (request.status === 400) {//введены неверные данные
                var msg = JSON.parse(request.responseText);
                alert(msg.message);
            }
            else console.log('Что-то пошло не так');
            PostManager.getPosts();//получить посты
        };
        request.send(JSON.stringify(user));
    }

    function Login(pereLogin) {
        if (SessionManager.FromSession() == null || pereLogin == 1) {//если в сессии нт пользователя или выполняется вход
            var user = {
                email: $("#loginemail").val(),
                password: $("#loginpassword").val()
            };
            let request = new XMLHttpRequest();
            request.open("POST", "api/Login");
            request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
            try {
                request.onload = function () {
                    if (request.status === 200) {//если авторизация прошла успешно
                        var checkuser = JSON.parse(request.responseText);
                        SessionManager.ToSession(checkuser);//записать пользователя в сессию
                        var string = "Выполнен вход: " + checkuser.email;
                        noteToHTML(string);//вывод сообщения об авторизации
                        PostManager.getPosts();
                        GetKarma();
                        $("#loginemail").val("");
                        $("#loginpassword").val("");
                        $(".close").click();//закрыть модальное окно
                    }
                    else if (request.status === 404) {//произошла ошибка
                        var msg = JSON.parse(request.responseText);
                        alert(msg.message);
                    }
                    else //назначить текущему пользователю роль гостя
                    {
                        noteToHTML("Вы вошли как гость, пожалуйста авторизируйтесь");
                        SessionManager.ToSession({ role: "guest" });
                    }
                };
            }
            catch (Exception) {
                console.log('Ошибка при авторизации');
            }
            request.send(JSON.stringify(user));
        }
        PostManager.getPosts();
    }

    function Logoff() {
        SessionManager.DeleteUserFromSession();
        SessionManager.ToSession({ role: "guest" });
        PostManager.getPosts();
        $(".close").click();
    }

    function GetKarma() {
        if (SessionManager.FromSession().role != "guest") {
            var user = {
                id: SessionManager.FromSession().id
            };
            let request = new XMLHttpRequest();
            request.open("POST", "api/Karma");
            request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
            request.onload = function () {
                if (request.status === 200) {
                    var karmauser = JSON.parse(request.responseText);
                    document.getElementById('Karma').innerHTML = 'Ваш рейтинг: ' + karmauser.karma;
                }
            };
            request.send(JSON.stringify(user));
        }
    }

    return {
        Register: Register,
        Login: Login,
        Logoff: Logoff,
        GetKarma: GetKarma
    }
}();

var SessionManager = function () {
    function ToSession(User) {
        if (window.sessionStorage && window.localStorage) {
            sessionStorage.setItem("cur_us", JSON.stringify(User));
        }
        else {
            //объекты sessionStorage и localtorage не поддерживаются
        }
    }

    function FromSession() {
        if (window.sessionStorage && window.localStorage) {
            var data = {};
            if (sessionStorage.getItem("cur_us")) {
                data = JSON.parse(sessionStorage.getItem("cur_us"));
                return data;
            }
        }
        else {
            //объекты sessionStorage и localtorage не поддерживаются
        }
    }

    function DeleteUserFromSession() {
        if (window.sessionStorage && window.localStorage) {
            if (sessionStorage.getItem("cur_us")) {
                sessionStorage.removeItem("cur_us");
            }
        }
        else {
            //объекты sessionStorage и localtorage не поддерживаются
        }
    }
    return {
        ToSession: ToSession,
        FromSession: FromSession,
        DeleteUserFromSession: DeleteUserFromSession
    }
}();

var PostManager = function () {
    function getPosts() {
        $.ajax({
            url: uri,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                PostsToHTML(data);
            },
            error: function () {
                noteToHTML('ERROR o_0');
            }
        });
    }

    function createPost() {
        var CurrentUser = SessionManager.FromSession();
        if (CurrentUser.role != "guest") {//если текущий пользователь не гость
            var post = {
                headline: $("#PostHeadline").val(),
                textbody: $("#PostTextbody").val(),
                userId: CurrentUser.id
            };
            var request = new XMLHttpRequest();
            request.open("POST", uri);
            request.onload = function () {
                getPosts();
                $("#PostHeadline").val("");
                $("#PostTextbody").val("");
                $("#PostAuthor").val("");
                $(".close").click();
            };
            request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
            request.send(JSON.stringify(post));
        }
        else alert('You are not authorised!');
    }

    function PostsToHTML(posts) {//создание html кода поста в соответствии с ролью пользователя
        let postsHTML = "";
        var CurrentUser = SessionManager.FromSession();
        $.each(posts, function (index, post) {
            postsHTML += '<div class="d-flex align-items-center mx-auto" style="width:100%; margin:5px;">';
            postsHTML += '<div class="d-flex bd-highlight">';
            postsHTML += '<div class="d-flex flex-column bd-highlight mb-3">';
            postsHTML += '<div class="p-2 bd-highlight">';
            postsHTML += '<button data_id ="' + post.id + '" data_rating="' + post.rating + '" ';
            if (CurrentUser.role != "guest")
                postsHTML += 'onclick = "VoteManager.RateUp(this)" ';
            postsHTML += 'class="btn btn-sm" style = "background-color:transparent" type = "button" > <span aria-hidden="true">&#xe014;</span></button ></div > ';
            postsHTML += '<div class="p-2 bd-highlight">';
            postsHTML += '<p id="rate" class="card-text">' + post.rating + '</p></div>';
            postsHTML += '<div class="p-2 bd-highlight">';
            postsHTML += '<button data_id="' + post.id + '" data_rating="' + post.rating + '" ';
            if (CurrentUser.role != "guest")
                postsHTML += 'onclick = "VoteManager.RateDown(this)" ';
            postsHTML += 'class="btn btn-sm" style = "background-color:transparent" type = "button" > <span aria-hidden="true">&#xe015;</span></button ></div > ';
            postsHTML += '</div></div>';

            postsHTML += '<div class="d-flex flex-fill bd-highlight mb-3">';
            postsHTML += '<div class="card mx-auto" style="width: 100%;"><div class="card-body">';
            postsHTML += '<h5 class="card-title">' + post.headline + '</h5>';
            postsHTML += '<h6 class="card-subtitle mb-2 text-muted">' + post.author + '</h6>';
            postsHTML += '<p class="card-text">' + post.textbody + '</p>';
            postsHTML += '<div class="d-flex bd-highlight mb-3">';
            if (CurrentUser.role == "admin" || CurrentUser.id == post.userId) {
                postsHTML += '<div class="p-2 bd-highlight">';
                postsHTML += '<button id="updatePost" data-item="' + post.id + '" type="button" class="btn btn-outline-warning" style="background-color:transparent" data-toggle="modal" data-target="#newPostModal" onclick="PostManager.EditItem(this)"><span aria-hidden="true">&#x270f;</span>Изменить</button></div>';
                postsHTML += '<div class="p-2 bd-highlight">';
                postsHTML += '<button type="button" data-item="' + post.id + '" class="btn btn-outline-warning" style="background-color:transparent" onclick="PostManager.deletePost(this)"><span aria-hidden="true">&#x274c;</span>Удалить</button></div>';
            }
            postsHTML += '<div class="ml-auto p-2 bd-highlight">';
            postsHTML += '<button data-item="' + post.id + '" onclick="CommentManager.GetComments(this)" class="btn btn-warning" data-toggle="modal" data-target="#CommentsModal" type="submit">Комментарии</button></div>';
            if (CurrentUser.role != "guest") {
                postsHTML += '<div class="ml-auto p-2 bd-highlight">';
                postsHTML += '<button data-item="' + post.id + '" type="button" class="btn btn-outline-warning" data-toggle="modal" data-target="#newCommentModal" onclick="CommentManager.AddCommentItem(this)">+Комментарий</button></div>';
            }
            postsHTML += '</div></div></div></div></div>';
        });
        $("#PostsList").html(postsHTML);
    }

    function GetPost(id) {
        $.ajax({
            url: uri + id,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                $("#PostHeadline").val(data.headline);
                $("#PostTextbody").val(data.textbody);
                $("#PostAuthor").val(data.author);
            },
            error: function () {
                alert('ERROR o_0');
            }
        });
    }

    function EditItem(el) {
        setupdatevisible();
        var id = $(el).attr('data-item');
        postid = id;//запомнить id выбранного поста
        GetPost(id);//получение выбранного поста
    }

    function editPost(id) {
        var CurrentUser = SessionManager.FromSession();
        if (CurrentUser != null) {
            var post = {//обновление поста
                headline: $("#PostHeadline").val(),
                textbody: $("#PostTextbody").val(),
                userId: CurrentUser.id
            };
            var request = new XMLHttpRequest();//инициализация запроса
            request.open("PUT", uri + id);
            request.onload = function () {
                $("#PostHeadline").val("");
                $("#PostTextbody").val("");
                $("#PostAuthor").val("");
                getPosts();
                $(".close").click();
            };
            request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
            request.send(JSON.stringify(post));
        }
        else alert('You are not authorised!');
    }

    function deletePost(el) {
        var id = $(el).attr('data-item');
        let request = new XMLHttpRequest();
        request.open("DELETE", uri + id);
        request.onload = function () {
            if (request.status === 204) {
                getPosts();
            } else {
                noteToHTML("Неизвестная");
            }
        };
        request.send();
    }

    return {
        getPosts: getPosts,
        createPost: createPost,
        PostsToHTML: PostsToHTML,
        GetPost: GetPost,
        EditItem: EditItem,
        editPost: editPost,
        deletePost: deletePost
    }
}();

var VoteManager = function () {
    function RateUp(el) {//проголосовать за
        var id = $(el).attr('data_id');//получение ид поста
        var rating = $(el).attr('data_rating');//получение рейтинга поста
        var vote = {//создание переменной голоса
            postId: id,
            userId: SessionManager.FromSession().id,
            forOrAgainst: 1,
            rating: rating
        };
        VotePost(vote);
    }

    function RateDown(el) {//прогголосовать против
        var id = $(el).attr('data_id');//получить ид поста
        var rating = $(el).attr('data_rating');//получить рэйтинг поста
        var vote = {//создание переменной голоса
            postId: id,
            userId: SessionManager.FromSession().id,
            forOrAgainst: 0,
            rating: rating
        };
        VotePost(vote);
    }

    function VotePost(vote) {//запрос на обработку голоса
        var request = new XMLHttpRequest();
        request.open("POST", "/api/PostVotes/");
        request.onload = function () {
            if (request.status === 200 || request.status === 204) {
                PostManager.getPosts();//обновить посты
                AccountManager.GetKarma();
            }
            else console.log("vote fail");
        };
        request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        request.send(JSON.stringify(vote));
    }

    return {
        RateUp: RateUp,
        RateDown: RateDown,
        VotePost: VotePost
    }
}();

var CommentManager = function () {
    var postid;
    function AddCommentItem(el) {
        var id = $(el).attr('data-item');
        postid = id;
    }

    function getID() {
        return postid;
    }

    function AddComment() {
        var comment = {
            text: $("#CommentTextbody").val(),
            postId: getID(),
            userId: SessionManager.FromSession().id
        };
        var request = new XMLHttpRequest();
        request.open("POST", uricoms);
        request.onload = function () {
            $("#CommentTextbody").val("");
            $(".close").click();
        };
        request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        request.send(JSON.stringify(comment));
    }

    function GetComments(el) {
        var id = $(el).attr('data-item');
        $.ajax({
            url: uricoms,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                CommentsToHTML(data, id);
            },
            error: function () {
                alert('ERROR o_0');
            }
        });
    }

    function CommentsToHTML(comments, id) {
        let commentsHTML = "";
        var CurrentUser = SessionManager.FromSession();
        $.each(comments, function (index, comment) {
            if (id == comment.postId) {
                commentsHTML += '<div class="card mx-auto" style="width: 100%;"><div class="card-body">';
                commentsHTML += '<h5 class="card-title">' + comment.author + '</h5>';
                commentsHTML += '<p class="card-text">' + comment.text + '</p>';
                commentsHTML += '<div class="d-flex bd-highlight mb-3">';
                if (CurrentUser.role == "admin" || CurrentUser.id == comment.userId) {
                    commentsHTML += '<div class="p-2 bd-highlight">';
                    commentsHTML += '<button data-item="' + comment.id + '" data-item1="' + comment.postId + '" type="button" class="btn btn-outline-warning" style="background-color:transparent" onclick="CommentManager.DeleteComment(this)"><span aria-hidden="true">&#x274c;</span>Удалить</button></div>';
                }
                commentsHTML += '</div></div></div>';
            }
        });
        $("#commentsOnPost").html(commentsHTML);
    }

    function DeleteComment(el) {
        var id = $(el).attr('data-item');
        var postid = $(el).attr('data-item1');
        let request = new XMLHttpRequest();
        request.open("DELETE", uricoms + id, false);
        request.onload = function () {
            GetComments(postid);
        };
        request.send();
    }
    return {
        postid: postid,
        AddCommentItem: AddCommentItem,
        AddComment: AddComment,
        GetComments: GetComments,
        CommentsToHTML: CommentsToHTML,
        DeleteComment: DeleteComment
    }
}();

function setupdatevisible() {
    $("#addPostBut").css('display', 'none');
    $("#editPostBut").css('display', 'block');
}

function setaddvisible() {
    $("#addPostBut").css('display', 'block');
    $("#editPostBut").css('display', 'none');
}

function noteToHTML(notation) {
    let noteHTML = "";
    noteHTML += '<div class="alert alert-warning alert-dismissible fade show" role="alert">';
    noteHTML += '<div id="Note">' + notation + '</div>';
    noteHTML += '<button type="button" class="close" data-dismiss="alert" aria-label="Close">';
    noteHTML += '<span aria-hidden="true">&times;</span></button></div>';
    document.getElementById('Notations').innerHTML += noteHTML;
}
