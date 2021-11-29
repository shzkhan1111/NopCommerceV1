$(function () {
	$('.description').each(function (key, val) {

		var maxLength = 60;
		var myStr = $(this).html();
		if ($(this).attr('id') == 'homepageProd') {
			maxLength = 30;
		}
		if ($(this).attr('id') == 'homepageProd-sm') {
			maxLength = 10;
		}
		var url = $(this).parent().parent().children().children('a').attr('href');

		if ($.trim(myStr).length > maxLength) {
			var res = myStr.substring(0, maxLength);
			$(this).empty().html(res);
			$(this).append('... <a href="' + url + '" class="read-more">Read more</a>');
		}
	})

	$('.product-title').each(function (key, val) {

		var maxLength = 80;
		var myStr = $(this).html();
		if ($(this).attr('id') == 'homepageProd') {
			maxLength = 30;
		}
		if ($(this).attr('id') == 'homepageProd-sm') {
			maxLength = 10;
		}
		var url = $(this).parent().parent().children().children('a').attr('href');

		if ($.trim(myStr).length > maxLength) {
			var res = myStr.substring(0, maxLength);
			$(this).empty().html(res);
			//$(this).append('... <a href="' + url + '" class="read-more">Read more</a>');
		}
	})
})
