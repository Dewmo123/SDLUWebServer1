# SaveInfoServer
플레이어 정보 저장 서버입니다. 공부용이구요 아마 서노 1분기때 사용 예정임니다

DB 테이블 정보 입니다
````
create table player_login_data(
	player_id varchar(8) primary key,
    password varchar(20)
);
create table item_data(
	item_id int primary key auto_increment,
    item_name varchar(50) not null,
    item_type enum('weapon','armor','consumption','material'),
    max_stack int default 1
);
create table player_item_data(
	player_id varchar(8),
    item_id int,
    quantity int default 1,
    primary key (player_id,item_id),
    foreign key (player_id) references player_login_data(player_id) on delete cascade,
    foreign key(item_id) references items(item_id) on delete cascade
);````
