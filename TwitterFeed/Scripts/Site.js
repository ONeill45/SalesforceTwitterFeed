$(document).ready(function () {
    setTimeout(refreshTweets, 60000);
    $(document).on('click', "#filterTweets", function () {
        var text = $('#searchBar').val();
        filterTweets(text);
        $('#unfilter').removeAttr('hidden');
    });
    $(document).on('click', "#unfilter", function () {
        $.ajax({
            url: rootUrl + 'Home/UnfilterTweets',
            type: 'POST',
            dataType: 'text',
            failure: function(xhr, status, error){

            },
            success: function (html) {
                $('#tweets').empty();
                $('#tweets').append(html);
            }
        });
        $('#unfilter').attr('hidden', true);
        $('#searchBar').val('');
    });
});
function refreshTweets() {
    $.ajax({
        url: rootUrl + 'Home/RefreshTweets',
        type: 'POST',
        dataType: 'text',
        failure: function (xhr, status, error) {
            alert('Unable to refresh tweets at this time');
        },
        success: function (html) {
            $('#tweets').empty();
            $('#tweets').append(html);
        }
    });
};
function filterTweets(text) {
    $.ajax({
        url: rootUrl + 'Home/FilterTweetsOnPage',
        contentType: 'application/json;charset=UTF-8',
        type: 'POST',
        data: JSON.stringify({ 'search': text }),
        dataType: 'text',
        failure: function (xhr, status, error) {
            alert('Cannot filter at this time. Please try again');
        },
        success: function (html) {
            $('#tweets').empty();
            $('#tweets').append(html);
        }
    });
};
