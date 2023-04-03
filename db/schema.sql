create table if not exists "group"
(
    id         bigint generated by default as identity
        constraint groupdb_pkey
            primary key,
    created_at timestamp with time zone default now() not null,
    group_id   text,
    group_name text
);

create table if not exists prompt
(
    id          bigint generated by default as identity
        constraint promptdb_pkey
            primary key,
    created_at  timestamp with time zone default now() not null,
    prompt_text text                                   not null,
    group_fk    bigint                                 not null
        references "group"
            on delete cascade,
    token_size  bigint                                 not null
);

create table if not exists message
(
    id           bigint generated by default as identity
        constraint messagedb_pkey
            primary key,
    created_at   timestamp with time zone default now() not null,
    prompt_fk    bigint                                 not null
        references prompt
            on delete cascade,
    message_type text,
    "message_Id" text,
    "user_Id"    text,
    user_name    text,
    phone        text,
    message      text,
    token_size   bigint
);