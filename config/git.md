
## Can NOT connect to 127.0.0.1?
1. `git remote -v` to see if remote url is correct
2. `cat /etc/hosts` to see if host is correct
3. `git config --global --list` to see if git proxy is correct
4. `env|grep -i proxy` to see if there has https proxy, and use `unset https_proxy_name` to remove them.

## set/unset proxy
git config --global https.proxy http://127.0.0.1:1080
git config --global https.proxy https://127.0.0.1:1080
git config --global --unset http.proxy
git config --global --unset https.proxy

## git log
```
git config --global alias.lm  "log --no-merges --color --date=format:'%Y-%m-%d %H:%M:%S' --author='fanfeilong' --pretty=format:'%Cred%h%Creset -%C(yellow)%d%Cblue %s %Cgreen(%cd) %C(bold blue)<%an>%Creset' --abbrev-commit"

git config --global alias.lms  "log --no-merges --color --stat --date=format:'%Y-%m-%d %H:%M:%S' --author='fanfeilong' --pretty=format:'%Cred%h%Creset -%C(yellow)%d%Cblue %s %Cgreen(%cd) %C(bold blue)<%an>%Creset' --abbrev-commit"

git config --global alias.ls "log --no-merges --color --graph --date=format:'%Y-%m-%d %H:%M:%S' --pretty=format:'%Cred%h%Creset -%C(yellow)%d%Cblue %s %Cgreen(%cd) %C(bold blue)<%an>%Creset' --abbrev-commit"

git config --global alias.lss "log --no-merges --color --stat --graph --date=format:'%Y-%m-%d %H:%M:%S' --pretty=format:'%Cred%h%Creset -%C(yellow)%d%Cblue %s %Cgreen(%cd) %C(bold blue)<%an>%Creset' --abbrev-commit"
```

## references
- [git - 简明指南](http://rogerdudler.github.io/git-guide/index.zh.html)
- [常用 Git 命令清单](http://www.ruanyifeng.com/blog/2015/12/git-cheat-sheet.html)
- [Commit Often, Perfect Later, Publish Once: Git Best Practices](https://sethrobertson.github.io/GitBestPractices/)
- [Understanding the GitHub Flow](https://guides.github.com/introduction/flow/)
- [A successful Git branching model](http://nvie.com/posts/a-successful-git-branching-model/)
- [How to Write a Git Commit Message](http://chris.beams.io/posts/git-commit/)
- [Keeping Commit Histories Clean](https://www.reviewboard.org/docs/codebase/dev/git/clean-commits/)
- [What's the best visual merge tool for Git?](http://stackoverflow.com/questions/137102/whats-the-best-visual-merge-tool-for-git)
- [visual diff](http://meldmerge.org/)
- [the core conception of git](https://lufficc.com/blog/the-core-conception-of-git)
- [Git from the inside out](https://maryrosecook.com/blog/post/git-from-the-inside-out)



