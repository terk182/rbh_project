var bricklayer = new Bricklayer(document.querySelector('.bricklayer'));
var xhr;
$(document).ready(function () {
    moment.locale('th');
    $('#mainScreen').hide();
    //addDeals();

    var waypoint = new Waypoint({
        element: document.getElementById('more'),
        handler: function (direction) {
            if (direction == 'down') {
                if ($('#alldeal').val() == 'false') {
                    $('#waiting').show();
                    addDeals();
                }
            }
        },
        offset: 'bottom-in-view'
    });

    $('.check-deal').change(function () {
        xhr.abort();
        $('#page').val('1');
        bricklayer.destroy();
        $('.bricklayer').html('');
        bricklayer = new Bricklayer(document.querySelector('.bricklayer'));
        $('#waiting').show();
        addDeals();
    });
});


var newBox = function(data, content, date, dealType, feedId) {
    var dom = '';
    dom += '<div class="card card-Gogojii">';
    if (data.subattachments) {
        console.log('slide == ' + date);
        dom += '<div class="">';
        dom += '<div id="' + feedId + '" class="carousel slide" data-ride="carousel" data-interval="4000">';
        dom += '<ol class="carousel-indicators">';
        for (var i = 0; i < data.subattachments.data.length; i++) {
            dom += '<li data-target="#' + feedId + '" data-slide-to="' + i + '" ' + (i == 0 ? 'class="active"' : '') + '></li>';
        }
        dom += '</ol>';
        dom += '<div class="carousel-inner">';
        for (var i = 0; i < data.subattachments.data.length; i++) {
            dom += '<div class="carousel-item ' + (i == 0 ? 'active' : '') + '">';
            dom += '<img class="w-100" src="' + data.subattachments.data[i].media.image.src + '" />';
            dom += '</div>';
        }
        dom += '</div>';
        dom += '</div>';
        dom += '</div>';
    }
    else {
        dom += '<img class="card-img-top card-img-Gogojii" src="' + data.media.image.src + '">';
        if (data.type != 'photo') {
            console.log(data);
        }
    }
    dom += '<div class="card-body">';
    
    dom += '<table style="width: 100%">';
    dom += '<tr>';
    dom += '<td>';
    dom += '<img src="https://graph.facebook.com/v3.1/791673117561032/picture" class="rounded-circle" style="width:40px;" />';
    dom += '</td>';
    dom += '<td>';
    dom += '<div><strong class="font-14">ไปเที่ยวกันมั้ย by Gogojii</strong></div>';
    dom += '<div class="font-12">' + date + '</div>';
    dom += '</td>';
    dom += '<td style="text-align: right;">';
    dom += '<span class="badge badge-pill badge-secondary">';
    dom += '<img src="/Images/deals_icon/star.png" class="img-fluid" style="width: 16px; margin-right: 3px;" />';
    dom += dealType;
    dom += '</span>';
    dom += '</td>';
    dom += '</tr>';
    dom += '</table>';
    dom += '<hr />';
    dom += '<p class="card-text font-14">' + linkify(content.replace(/(?:\r\n|\r|\n)/g, '  <br />')) + '</p>';
    dom += '</div>';
    dom += '</div>';
    return $.parseHTML(dom);
};

var addDeals = function () {
    var page = parseInt($('#page').val());
    var cat = '';
    var c = $('input:checkbox.check-deal');
    for (var i = 0; i < c.length; i++) {
        if (c[i].checked) {
            cat += (cat === '' ? '' : ',') + c[i].value;
            $("#badge_" + c[i].value).removeClass("badge-secondary").addClass("badge-warning");
        } else {
            $("#badge_" + c[i].value).removeClass("badge-warning").addClass("badge-secondary");
        }
    }

    xhr = $.get($('#dealURL').val() + '/?item=12&page=' + page + '&catdeal=' + cat,
        function (data) {
            $('#waiting').hide();
            var fbdata = data.fbFeedList.fbdata;
            for (var i = 0; i < fbdata.length; i++) {
                var fbDescription = JSON.parse(fbdata[i].fbDescription);
                var date = moment(fbdata[i].fbCreateDate, 'M/D/YYYY hh:mm:ss A');
                var box = newBox(fbDescription.data[0], fbdata[i].fbMessage,
                    date.format('DD MMM YY HH:mm') + ' น.', fbdata[i].fbType, fbdata[i].fbFeedId);
                bricklayer.append(box);
            }
            $('.carousel').carousel({
                interval: 4000
            })
            page = page + 1;
            $('#page').val(page);
            if (data.fbFeedList.numberOfPage == data.fbFeedList.pageIndex) {
                $('#alldeal').val('true');
            }
            Waypoint.refreshAll();
        });
};


var linkify = function (inputText) {
    var replacedText, replacePattern1, replacePattern2, replacePattern3;

    //URLs starting with http://, https://, or ftp://
    replacePattern1 = /(\b(https?|ftp):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/gim;
    replacedText = inputText.replace(replacePattern1, '<a href="$1" target="_blank">$1</a>');

    //URLs starting with "www." (without // before it, or it'd re-link the ones done above).
    replacePattern2 = /(^|[^\/])(www\.[\S]+(\b|$))/gim;
    replacedText = replacedText.replace(replacePattern2, '$1<a href="http://$2" target="_blank">$2</a>');

    //Change email addresses to mailto:: links.
    replacePattern3 = /(([a-zA-Z0-9\-\_\.])+@[a-zA-Z\_]+?(\.[a-zA-Z]{2,6})+)/gim;
    replacedText = replacedText.replace(replacePattern3, '<a href="mailto:$1">$1</a>');

    return replacedText;
};