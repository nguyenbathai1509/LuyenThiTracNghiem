// subject

$(document).ready(function () {
    // Khi gõ tìm kiếm
    $("#searchInput").on("keyup", function () {
        var keyword = $(this).val();

        if (keyword.trim() === "") {
            // Input rỗng -> load lại Index
            $.get('/Admin/Subject/Index', function (result) {
                var html = $(result).find("#subjectList").html();
                $("#subjectList").html(html);
            });
        } else {
            // Có từ khóa -> gọi Search
            $.get('/Admin/Subject/Search', { searchTerm: keyword, page: 1 }, function (result) {
                $("#subjectList").html(result);
            });
        }
    });

    // Phân trang (cho cả Index + Search)
    $(document).on("click", ".page-link", function (e) {
        e.preventDefault();
        var page = $(this).data("page");
        var keyword = $("#searchInput").val();

        if (keyword.trim() === "") {
            $.get('/Admin/Subject/Index', { page: page }, function (result) {
                var html = $(result).find("#subjectList").html();
                $("#subjectList").html(html);
            });
        } else {
            $.get('/Admin/Subject/Search', { searchTerm: keyword, page: page }, function (result) {
                $("#subjectList").html(result);
            });
        }
    });
});


// exam
function loadExams(page = 1) {
    var keyword = $("#searchInput").val();
    var subjectId = $("#subjectFilter").val();
    var examType = $("#examTypeFilter").val();

    $.get('/Admin/Exam/LoadData', { 
        searchTerm: keyword, 
        subjectId: subjectId, 
        examType: examType,
        page: page 
    }, function (result) {
        $("#examList").html(result);
    });
}

$(document).ready(function () {
    // Load mặc định
    loadExams();

    // Tìm kiếm bằng enter hoặc gõ
    $("#searchInput").on("keyup", function () {
        loadExams(1);
    });

    // Nút lọc
    $("#filterBtn").on("click", function () {
        loadExams(1);
    });

    // Phân trang
    $(document).on("click", ".page-link", function (e) {
        e.preventDefault();
        var page = $(this).data("page");
        loadExams(page);
    });
});

// question
function loadQuestions(page = 1) {
    var keyword = $("#searchInput").val();
    var subjectId = $("#subjectFilter").val();
    var level = $("#questionLevelFilter").val();

    $.get('/Admin/QuestionAndAnswer/LoadData', {
        searchTerm: keyword,
        subjectId: subjectId,
        level: level,   // phải trùng với tên tham số trong Controller
        page: page
    }, function (result) {
        $("#questionList").html(result); // phải trùng với div ngoài View
    });
}

$(document).ready(function () {
    // Load mặc định
    loadQuestions();

    // Tìm kiếm realtime
    $("#searchInput").on("keyup", function () {
        loadQuestions(1);
    });

    // Nút lọc
    $("#filterBtn").on("click", function () {
        loadQuestions(1);
    });

    // Phân trang
    $(document).on("click", ".page-link", function (e) {
        e.preventDefault();
        var page = $(this).data("page");
        if (page) {
            loadQuestions(page);
        }
    });
});
