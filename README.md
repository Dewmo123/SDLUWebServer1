# SDLUWebServer1
플레이어 정보 저장 서버입니다. 공부용이구요 아마 서노 1분기때 사용 예정임니다

DB 테이블 정보 입니다
````Sql
use sdlu_db;
create table player_login_data(
	player_id varchar(8) primary key,
    password varchar(20)
);
create table item_data(
    item_name varchar(50) not null,
    item_type enum('dictionary','material'),
    primary key (item_name)
);
create table player_item_data(
	player_id varchar(8),
    item_name varchar(50),
    quantity int default 1,
    primary key (player_id,item_name),
    foreign key (player_id) references player_login_data(player_id) on delete cascade,
    foreign key(item_name) references item_data(item_name) on delete cascade
);
create table auction(
	player_id varchar(8),
    quantity int default 1,
    price_per_unit int default 1,
    item_name varchar(50),
    primary key (player_id,item_name,price_per_unit),
    foreign key(player_id) references player_login_data(player_id) on delete cascade,
    foreign key(item_name) references item_data(item_name) on delete cascade
);
create table player_data(
	player_id varchar(8),
    gold int,
    dictionary text,
    weapon_level int default 1,
    armor_level int default 1,
    primary key (player_id),
    foreign key (player_id) references player_login_data(player_id) on delete cascade
);
````
