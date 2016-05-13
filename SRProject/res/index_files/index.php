/*
	全角・半角正規化スクリプト

	「表示」ボタンを押したときに、テキストボックス内の
	全角 / 半角文字を[[WP:NC]]に沿ったものに変換します。
 */

(function () {
var NORMALIZATION_TABLE = {
	//全角記号
	'！': '!', '＄': '$', '％': '%', '＊': '*', '＋': '+',
	'，': ',', '－': '-', '．': '.', '／': '/', '＾': '^',
	/* '：': ':', */ '；': ';', '？': '?', '＠': '@',
	//全角数字
	'０': '0', '１': '1', '２': '2', '３': '3', '４': '4',
	'５': '5', '６': '6', '７': '7', '８': '8', '９': '9',
	//全角アルファベット
	'ａ': 'a', 'ｂ': 'b', 'ｃ': 'c', 'ｄ': 'd', 'ｅ': 'e', 'ｆ': 'f', 'ｇ': 'g',
	'ｈ': 'h', 'ｉ': 'i', 'ｊ': 'j', 'ｋ': 'k', 'ｌ': 'l', 'ｍ': 'm', 'ｎ': 'n',
	'ｏ': 'o', 'ｐ': 'p', 'ｑ': 'q', 'ｒ': 'r', 'ｓ': 's', 'ｔ': 't', 'ｕ': 'u',
	'ｖ': 'v', 'ｗ': 'w', 'ｘ': 'x', 'ｙ': 'y', 'ｚ': 'z',
	'Ａ': 'A', 'Ｂ': 'B', 'Ｃ': 'C', 'Ｄ': 'D', 'Ｅ': 'E', 'Ｆ': 'F', 'Ｇ': 'G',
	'Ｈ': 'H', 'Ｉ': 'I', 'Ｊ': 'J', 'Ｋ': 'K', 'Ｌ': 'L', 'Ｍ': 'M', 'Ｎ': 'N',
	'Ｏ': 'O', 'Ｐ': 'P', 'Ｑ': 'Q', 'Ｒ': 'R', 'Ｓ': 'S', 'Ｔ': 'T', 'Ｕ': 'U',
	'Ｖ': 'V', 'Ｗ': 'W', 'Ｘ': 'X', 'Ｙ': 'Y', 'Ｚ': 'Z',
	//半角カナ記号
	'｡': '。', '｢': '「', '｣': '」', '･': '・', '､': '、',
	'ﾞ': '゛', 'ﾟ': '゜', 'ｰ': 'ー',
	//半角カナ
	'ｱ': 'ア', 'ｲ': 'イ', 'ｳ': 'ウ', 'ｴ': 'エ', 'ｵ': 'オ',
	'ｧ': 'ァ', 'ｨ': 'ィ', 'ｩ': 'ゥ', 'ｪ': 'ェ', 'ｫ': 'ォ',
	'ｶ': 'カ', 'ｷ': 'キ', 'ｸ': 'ク', 'ｹ': 'ケ', 'ｺ': 'コ',
	'ｻ': 'サ', 'ｼ': 'シ', 'ｽ': 'ス', 'ｾ': 'セ', 'ｿ': 'ソ',
	'ﾀ': 'タ', 'ﾁ': 'チ', 'ﾂ': 'ツ', 'ﾃ': 'テ', 'ﾄ': 'ト', 'ｯ': 'ッ',
	'ﾅ': 'ナ', 'ﾆ': 'ニ', 'ﾇ': 'ヌ', 'ﾈ': 'ネ', 'ﾉ': 'ノ',
	'ﾊ': 'ハ', 'ﾋ': 'ヒ', 'ﾌ': 'フ', 'ﾍ': 'ヘ', 'ﾎ': 'ホ',
	'ﾏ': 'マ', 'ﾐ': 'ミ', 'ﾑ': 'ム', 'ﾒ': 'メ', 'ﾓ': 'モ',
	'ﾔ': 'ヤ', 'ﾕ': 'ユ', 'ﾖ': 'ヨ', 'ｬ': 'ャ', 'ｭ': 'ュ', 'ｮ': 'ョ',
	'ﾗ': 'ラ', 'ﾘ': 'リ', 'ﾙ': 'ル', 'ﾚ': 'レ', 'ﾛ': 'ロ',
	'ﾜ': 'ワ', 'ｦ': 'ヲ', 'ﾝ': 'ン',
	//その他
	'~': '〜', '～': '〜', '　': ' '
};
var NORMALIZATION_TABLE_DAKUTEN = {
	'ウ゛': 'ヴ',
	'カ゛': 'ガ', 'キ゛': 'ギ', 'ク゛': 'グ', 'ケ゛': 'ゲ', 'コ゛': 'ゴ',
	'サ゛': 'ザ', 'シ゛': 'ジ', 'ス゛': 'ズ', 'セ゛': 'ゼ', 'ソ゛': 'ゾ',
	'タ゛': 'ダ', 'チ゛': 'ヂ', 'ツ゛': 'ヅ', 'テ゛': 'デ', 'ト゛': 'ド',
	'ハ゛': 'バ', 'ヒ゛': 'ビ', 'フ゛': 'ブ', 'ヘ゛': 'ベ', 'ホ゛': 'ボ',
	'ハ゜': 'パ', 'ヒ゜': 'ピ', 'フ゜': 'プ', 'ヘ゜': 'ペ', 'ホ゜': 'ポ'
};
function normalizeCharWidth(src) {
	//利用者（会話）名前空間と特別ページは除外
	if(src.search(/^\s*(利用者(‐会話)?|User( talk)?|特別|Special):/i) != -1)
		return src;
	
	src = src.replace(/./g, function(m) { return NORMALIZATION_TABLE[m] || m; });
	var reg = /([ウカキクケコサシスセソタチツテト]゛|[ハヒフヘホ][゛゜])/g;
	return src.replace(reg, function(m) { return NORMALIZATION_TABLE_DAKUTEN[m] || m; });
}

$(function() {
	var is_msie = window.attachEvent && !window.opera;
	var boxButtonPairs = [];
	function appendPair(text, submit) {
		if (text && submit) {
			boxButtonPairs.push([text, submit]);
		}
	}
	
	appendPair(
		document.getElementById('searchInput'),
		document.getElementById('searchGoButton')
	);
	
	// <inputbox> で追加される検索フォーム
	var additionalSearchBox = document.getElementById('searchbox');
	if(additionalSearchBox) {
		appendPair(
			additionalSearchBox.elements.namedItem('search'),
			additionalSearchBox.elements.namedItem('go')
		);
	}

	for(var i = 0; i < boxButtonPairs.length; i++) (function(inputBox, goButton) {
		if (!goButton) return;

		$(goButton).on('click', function(e) {
			inputBox.value = normalizeCharWidth(inputBox.value);
		});
		if(is_msie) { //MSIEでもEnterキーによる送信時にイベントを発生させる
			$(inputBox).on('keydown', function(e) {
				if(window.event.keyCode == 13) //13: Enter
					inputBox.value = normalizeCharWidth(inputBox.value);
			});
		}
	})(boxButtonPairs[i][0], boxButtonPairs[i][1]);
});

})();