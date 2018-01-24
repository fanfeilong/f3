## [mysql][mac]
- 配置文件路径：/usr/local/mysql/support-files/
- 拷贝cng到etc/my.cnf: `sudo cp /usr/local/mysql/my-default.cnf /etc/my.cnf`
- 重启：sudo /usr/local/mysql/support-files/mysql.server restart
- 允许其他机子访问：
    - my.cnf配置bind-address=0.0.0.0
    - 命令行登录数据库，修改权限：
        - update user set host='%' where user='root';
        - flush privileges;
        - select host, user from user;

## [mysql][centos]
- wget https://dev.mysql.com/get/mysql57-community-release-el7-11.noarch.rpm
- rpm --force -ivh mysql57-community-release-el7-11.noarch.rpm
- yum -y remove mysql-libs
- yum -y install mysql-community-server
- service mysqld restart
- systemctl enable mysqld

备注，修改新安装的mysql 5.7版本的root密码，打开非本地登陆
- grep 'temporary password' /var/log/mysqld.log

拿到初始root密码
- mysql -uroot -p
- set global validate_password_policy=0;
- set global validate_password_length=1;
- ALTER USER 'root'@'localhost' IDENTIFIED BY '123456';
- grant all PRIVILEGES on *.* to root@'%' identified by '123456';
- flush privileges

## [mysql][windows]
- oracle 官方下载（需要注册账号）：
    - https://dev.mysql.com/downloads/instanller/
- 安装visual c++ 2013 redistubuted: 
    - https://support.microsoft.com/en-us/help/3179560/update-for-visual-c-2013-and-visual-c-redistributable-package
- 安装visual c++ 2015 redistributed:
    - https://www.microsoft.com/zh-cn/download/details.aspx?id=48145
- 使用installer一键安装，忽略不满足条件的组件（mysql for excel之类）
    - 最后会让设置root密码

## [mysql][atomic/concurrency]
- [MySQL and an atomic 'check ... on none insert'](https://blog.fastmail.com/2017/12/09/mysql-lock-nonexistent-row/)
- [MySQL 加锁处理分析](https://github.com/hedengcheng/tech/blob/master/database/MySQL/MySQL%20%E5%8A%A0%E9%94%81%E5%A4%84%E7%90%86%E5%88%86%E6%9E%90.pdf)




