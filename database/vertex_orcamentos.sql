CREATE EXTENSION IF NOT EXISTS "pgcrypto";

CREATE SCHEMA IF NOT EXISTS pessoas;
CREATE SCHEMA IF NOT EXISTS documentos;
CREATE SCHEMA IF NOT EXISTS estoque;

DROP TABLE IF EXISTS documentos.tb_orcamento_produto;
DROP TABLE IF EXISTS documentos.tb_orcamento;
DROP TABLE IF EXISTS pessoas.tb_usuario;
DROP TABLE IF EXISTS estoque.tb_produto;
DROP TABLE IF EXISTS pessoas.tb_pessoa;

CREATE TABLE pessoas.tb_pessoa
(
    id               UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    cpf              VARCHAR(14)  NOT NULL,
    primeiro_nome    VARCHAR(20)  NOT NULL,
    telefone         VARCHAR(20)  NOT NULL,
    email            VARCHAR(120) NOT NULL,
    endereco         VARCHAR(100) NOT NULL,
    tipo_pessoa      VARCHAR(20)  NOT NULL DEFAULT 'Cliente',
    status           VARCHAR(20)  NOT NULL DEFAULT 'Ativo',
    data_criacao     TIMESTAMPTZ  NOT NULL DEFAULT now(),
    data_modificacao TIMESTAMPTZ
);

CREATE TABLE pessoas.tb_usuario
(
    id                      UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    id_pessoa               UUID        NOT NULL REFERENCES pessoas.tb_pessoa(id),
    login                   VARCHAR(36) NOT NULL UNIQUE,
    senha                   VARCHAR(50) NOT NULL,
    cargo                   VARCHAR(36) NOT NULL,
    tentativas_restantes    INTEGER     NOT NULL DEFAULT 3,
    max_tentativas_segundos INTEGER     NOT NULL DEFAULT 300,
    datahora_ultimo_acesso  TIMESTAMPTZ NOT NULL DEFAULT now(),
    usuario_ativo           BOOLEAN     NOT NULL DEFAULT true,
    data_criacao            TIMESTAMPTZ NOT NULL DEFAULT now(),
    data_modificacao        TIMESTAMPTZ
);

CREATE TABLE estoque.tb_produto
(
    id                 UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nome_produto       VARCHAR(80)   NOT NULL,
    quantidade_estoque NUMERIC(10,2) NOT NULL,
    unidade_medida     VARCHAR(20)   NOT NULL,
    valor_unitario     NUMERIC(10,2) NOT NULL,
    categoria_produto  VARCHAR(20)   NOT NULL,
    produto_ativo      BOOLEAN       NOT NULL DEFAULT true,
    data_criacao       TIMESTAMPTZ   NOT NULL DEFAULT now(),
    data_modificacao   TIMESTAMPTZ
);

CREATE TABLE documentos.tb_orcamento
(
    id                 UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    id_usuario         UUID NOT NULL REFERENCES pessoas.tb_usuario(id),
    id_cliente         UUID NOT NULL REFERENCES pessoas.tb_pessoa(id),
    id_produto         UUID NOT NULL REFERENCES estoque.tb_produto(id),
    codigo             VARCHAR(20)   NOT NULL,
    status             VARCHAR(20)   NOT NULL DEFAULT 'Pendente',
    valor_total        NUMERIC(12,2) NOT NULL DEFAULT 0,
    condicao_pagamento VARCHAR(50)   NOT NULL DEFAULT 'A vista',
    validade_dias      INTEGER       NOT NULL DEFAULT 15,
    frete              NUMERIC(10,2) NOT NULL DEFAULT 0,
    observacoes        VARCHAR(300)  NOT NULL DEFAULT '',
    data_criacao       TIMESTAMPTZ   NOT NULL DEFAULT now(),
    data_modificacao   TIMESTAMPTZ
);

CREATE TABLE documentos.tb_orcamento_produto
(
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    id_orcamento        UUID NOT NULL REFERENCES documentos.tb_orcamento(id),
    id_produto          UUID NOT NULL REFERENCES estoque.tb_produto(id),
    quantidade          NUMERIC(10,2) NOT NULL DEFAULT 1,
    valor_unitario      NUMERIC(10,2) NOT NULL DEFAULT 0,
    desconto_percentual NUMERIC(5,2)  NOT NULL DEFAULT 0,
    observacao          VARCHAR(200)  NOT NULL DEFAULT '',
    data_criacao        TIMESTAMPTZ   NOT NULL DEFAULT now(),
    data_modificacao    TIMESTAMPTZ
);

INSERT INTO pessoas.tb_pessoa (id, cpf, primeiro_nome, telefone, email, endereco, tipo_pessoa, status, data_criacao) VALUES
('00000000-0000-0000-0000-000000000001','000.000.000-00','Admin','(83) 90000-0000','admin@vertex.com','Joao Pessoa - PB','Usuario','Ativo',now()),
('00000000-0000-0000-0000-000000000002','111.111.111-11','Carlos','(83) 98111-1200','carlos@vertex.com','Joao Pessoa - PB','Usuario','Ativo',now()),
('00000000-0000-0000-0000-000000000003','222.222.222-22','Marina','(83) 98222-1300','marina@vertex.com','Cabedelo - PB','Usuario','Ativo',now()),
('10000000-0000-0000-0000-000000000001','123.456.789-10','TechPlay','(83) 98810-1010','compras@techplay.com','Av. Gamer, 120','Cliente','Ativo',now()),
('10000000-0000-0000-0000-000000000002','234.567.890-11','PixelZone','(83) 98820-2020','financeiro@pixelzone.com','Rua RGB, 45','Cliente','Ativo',now()),
('10000000-0000-0000-0000-000000000003','345.678.901-12','ArenaPC','(83) 98830-3030','contato@arenapc.com','Shopping Tech, loja 18','Cliente','Ativo',now()),
('10000000-0000-0000-0000-000000000004','456.789.012-13','ByteHouse','(83) 98840-4040','ti@bytehouse.com','Rua dos Setups, 77','Cliente','Pendente',now()),
('10000000-0000-0000-0000-000000000005','567.890.123-14','GameLab','(83) 98850-5050','pedidos@gamelab.com','Av. E-sports, 900','Cliente','Bloqueado',now()),
('10000000-0000-0000-0000-000000000006','678.901.234-15','NexusLAN','(83) 98860-6060','nexus@lan.com','Rua Hardware, 300','Cliente','Ativo',now());

INSERT INTO pessoas.tb_usuario (id, id_pessoa, login, senha, cargo, tentativas_restantes, max_tentativas_segundos, datahora_ultimo_acesso, usuario_ativo, data_criacao) VALUES
('20000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000001','admin','JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=','Admin',3,300,now(),true,now()),
('20000000-0000-0000-0000-000000000002','00000000-0000-0000-0000-000000000002','carlos','JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=','Funcionario',3,300,now(),true,now()),
('20000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000003','marina','JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=','Funcionario',3,300,now(),true,now());

INSERT INTO estoque.tb_produto (id, nome_produto, quantidade_estoque, unidade_medida, valor_unitario, categoria_produto, produto_ativo, data_criacao) VALUES
('30000000-0000-0000-0000-000000000001','Mouse Gamer RGB 12000 DPI',34,'un',189.90,'Perifericos',true,now()),
('30000000-0000-0000-0000-000000000002','Teclado Mecanico Switch Blue',18,'un',349.90,'Perifericos',true,now()),
('30000000-0000-0000-0000-000000000003','Monitor Gamer 27 165Hz',7,'un',1599.90,'Monitores',true,now()),
('30000000-0000-0000-0000-000000000004','Headset Surround 7.1',4,'un',299.90,'Audio',true,now()),
('30000000-0000-0000-0000-000000000005','Placa de Video RTX 4060',3,'un',2299.90,'Pecas',true,now()),
('30000000-0000-0000-0000-000000000006','SSD NVMe 1TB',26,'un',449.90,'Armazenamento',true,now()),
('30000000-0000-0000-0000-000000000007','Fonte 750W 80 Plus Gold',11,'un',549.90,'Pecas',true,now()),
('30000000-0000-0000-0000-000000000008','Gabinete Gamer Mid Tower',9,'un',399.90,'Gabinetes',true,now()),
('30000000-0000-0000-0000-000000000009','Cadeira Gamer Ergonomica',6,'un',1199.90,'Moveis',true,now()),
('30000000-0000-0000-0000-000000000010','Water Cooler 240mm RGB',2,'un',489.90,'Refrigeracao',true,now()),
('30000000-0000-0000-0000-000000000011','Mousepad XL Speed',40,'un',89.90,'Acessorios',true,now()),
('30000000-0000-0000-0000-000000000012','Webcam Full HD Streaming',5,'un',259.90,'Streaming',true,now());

INSERT INTO documentos.tb_orcamento (id, id_usuario, id_cliente, id_produto, codigo, status, valor_total, condicao_pagamento, validade_dias, frete, observacoes, data_criacao, data_modificacao) VALUES
('40000000-0000-0000-0000-000000000001','20000000-0000-0000-0000-000000000002','10000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000001','PED-1001','Pago',4209.20,'Cartao',7,40,'Setup gamer completo',now() - interval '18 days',now() - interval '18 days' + interval '24 minutes'),
('40000000-0000-0000-0000-000000000002','20000000-0000-0000-0000-000000000002','10000000-0000-0000-0000-000000000002','30000000-0000-0000-0000-000000000003','PED-1002','Pago',3249.80,'Pix',7,50,'Upgrade de monitores',now() - interval '12 days',now() - interval '12 days' + interval '16 minutes'),
('40000000-0000-0000-0000-000000000003','20000000-0000-0000-0000-000000000003','10000000-0000-0000-0000-000000000003','30000000-0000-0000-0000-000000000006','PED-1003','Pendente',1979.60,'Boleto',10,30,'Reposicao de armazenamento',now() - interval '8 days',null),
('40000000-0000-0000-0000-000000000004','20000000-0000-0000-0000-000000000003','10000000-0000-0000-0000-000000000004','30000000-0000-0000-0000-000000000005','PED-1004','Cancelado',2299.90,'Cartao',7,0,'Compra cancelada pelo cliente',now() - interval '6 days',now() - interval '6 days' + interval '7 minutes'),
('40000000-0000-0000-0000-000000000005','20000000-0000-0000-0000-000000000002','10000000-0000-0000-0000-000000000006','30000000-0000-0000-0000-000000000002','PED-1005','Pago',2369.30,'Pix',7,25,'Perifericos para lan house',now() - interval '2 days',now() - interval '2 days' + interval '19 minutes'),
('40000000-0000-0000-0000-000000000006','20000000-0000-0000-0000-000000000003','10000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000011','PED-1006','Pago',899.00,'Pix',7,0,'Acessorios para setup',now() - interval '1 days',now() - interval '1 days' + interval '11 minutes');

INSERT INTO documentos.tb_orcamento_produto (id_orcamento, id_produto, quantidade, valor_unitario, desconto_percentual, observacao, data_criacao) VALUES
('40000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000001',8,189.90,0,'Mouse para setup gamer',now() - interval '18 days'),
('40000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000002',4,349.90,0,'Teclados mecanicos',now() - interval '18 days'),
('40000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000004',4,299.90,0,'Headsets 7.1',now() - interval '18 days'),
('40000000-0000-0000-0000-000000000002','30000000-0000-0000-0000-000000000003',2,1599.90,0,'Monitores 165Hz',now() - interval '12 days'),
('40000000-0000-0000-0000-000000000003','30000000-0000-0000-0000-000000000006',4,449.90,0,'SSDs NVMe',now() - interval '8 days'),
('40000000-0000-0000-0000-000000000003','30000000-0000-0000-0000-000000000011',2,89.90,0,'Mousepads XL',now() - interval '8 days'),
('40000000-0000-0000-0000-000000000004','30000000-0000-0000-0000-000000000005',1,2299.90,0,'GPU RTX 4060',now() - interval '6 days'),
('40000000-0000-0000-0000-000000000005','30000000-0000-0000-0000-000000000002',6,349.90,0,'Teclados para estacoes',now() - interval '2 days'),
('40000000-0000-0000-0000-000000000005','30000000-0000-0000-0000-000000000011',3,89.90,0,'Mousepads',now() - interval '2 days'),
('40000000-0000-0000-0000-000000000006','30000000-0000-0000-0000-000000000011',10,89.90,0,'Acessorios',now() - interval '1 days');
