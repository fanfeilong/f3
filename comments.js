(function(){
  var students = document.querySelector(".am-u-md-12>.am-u-md-12")
  var links = students.querySelectorAll(".am-list-item-hd>a");
  var i=0;
  var timer = setInterval(function(){
    i=i+1; 
    if(i<links.length){
    	//links[i].click(); 
      var link = links[i];
      var url = link.getAttribute("href");
      window.popup = window.open(url);
      window.popup.onload = function () {
        alert(url)
        var doc = x.document;
        var title = doc.querySelector("#cb_post_title_url").textContent;
        //if(title.indexOf("PSP")!==-1){
          var msg = "继续加油，多多小结软件工程知识点，并运用在项目开发中";
          
          var comment = doc.getElementById("tbCommentBody");
          comment.value = prompt(title,msg);
          
          var submit = doc.getElementById("btn_comment_submit");
          //submit.click();
        //}
      };
    } else {
        clearInterval(timer);
        alert("done");
    }
  },1);
})()


var title = document.querySelector("#cb_post_title_url").textContent;
var msg = "继续加油，多多小结软件工程知识点，并运用在项目开发中";
document.getElementById("tbCommentBody").value = prompt(title,msg);
document.getElementById("btn_comment_submit").click();


