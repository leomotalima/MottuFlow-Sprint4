using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MottuFlowApi.Migrations
{
    /// <inheritdoc />
    public partial class Iniciando : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "funcionario",
                columns: table => new
                {
                    id_funcionario = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    cpf = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: false),
                    cargo = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    telefone = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    senha = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_funcionario", x => x.id_funcionario);
                });

            migrationBuilder.CreateTable(
                name: "patio",
                columns: table => new
                {
                    id_patio = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    endereco = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    capacidade_maxima = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patio", x => x.id_patio);
                });

            migrationBuilder.CreateTable(
                name: "camera",
                columns: table => new
                {
                    id_camera = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    status_operacional = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    localizacao_fisica = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    id_patio = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_camera", x => x.id_camera);
                    table.ForeignKey(
                        name: "FK_camera_patio_id_patio",
                        column: x => x.id_patio,
                        principalTable: "patio",
                        principalColumn: "id_patio",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "moto",
                columns: table => new
                {
                    id_moto = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    placa = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    modelo = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    fabricante = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    ano = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    id_patio = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    localizacao_atual = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_moto", x => x.id_moto);
                    table.ForeignKey(
                        name: "FK_moto_patio_id_patio",
                        column: x => x.id_patio,
                        principalTable: "patio",
                        principalColumn: "id_patio",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aruco_tag",
                columns: table => new
                {
                    id_tag = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    codigo = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    id_moto = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aruco_tag", x => x.id_tag);
                    table.ForeignKey(
                        name: "FK_aruco_tag_moto_id_moto",
                        column: x => x.id_moto,
                        principalTable: "moto",
                        principalColumn: "id_moto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "localidade",
                columns: table => new
                {
                    id_localidade = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    data_hora = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ponto_referencia = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    id_moto = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    id_patio = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    id_camera = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_localidade", x => x.id_localidade);
                    table.ForeignKey(
                        name: "FK_localidade_camera_id_camera",
                        column: x => x.id_camera,
                        principalTable: "camera",
                        principalColumn: "id_camera",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_localidade_moto_id_moto",
                        column: x => x.id_moto,
                        principalTable: "moto",
                        principalColumn: "id_moto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_localidade_patio_id_patio",
                        column: x => x.id_patio,
                        principalTable: "patio",
                        principalColumn: "id_patio",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "registro_status",
                columns: table => new
                {
                    id_status = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    tipo_status = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    descricao = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    data_status = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    id_moto = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    id_funcionario = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registro_status", x => x.id_status);
                    table.ForeignKey(
                        name: "FK_registro_status_funcionario_id_funcionario",
                        column: x => x.id_funcionario,
                        principalTable: "funcionario",
                        principalColumn: "id_funcionario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_registro_status_moto_id_moto",
                        column: x => x.id_moto,
                        principalTable: "moto",
                        principalColumn: "id_moto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_aruco_tag_id_moto",
                table: "aruco_tag",
                column: "id_moto");

            migrationBuilder.CreateIndex(
                name: "IX_camera_id_patio",
                table: "camera",
                column: "id_patio");

            migrationBuilder.CreateIndex(
                name: "IX_localidade_id_camera",
                table: "localidade",
                column: "id_camera");

            migrationBuilder.CreateIndex(
                name: "IX_localidade_id_moto",
                table: "localidade",
                column: "id_moto");

            migrationBuilder.CreateIndex(
                name: "IX_localidade_id_patio",
                table: "localidade",
                column: "id_patio");

            migrationBuilder.CreateIndex(
                name: "IX_moto_id_patio",
                table: "moto",
                column: "id_patio");

            migrationBuilder.CreateIndex(
                name: "IX_registro_status_id_funcionario",
                table: "registro_status",
                column: "id_funcionario");

            migrationBuilder.CreateIndex(
                name: "IX_registro_status_id_moto",
                table: "registro_status",
                column: "id_moto");

            // Inserção de dados iniciais
            migrationBuilder.Sql(@"
                INSERT INTO ""JOAO"".""funcionario"" (""nome"", ""cpf"", ""cargo"", ""telefone"", ""email"", ""senha"")
                VALUES ('Admin', '000.000.000-00', 'Administrador', '(00)00000-0000', 'admin@mottu.com', 'adminmottu123')");

            migrationBuilder.Sql(@"
                INSERT INTO ""JOAO"".""patio"" (""nome"", ""endereco"", ""capacidade_maxima"")
                VALUES ('Pátio Central', 'Rua Exemplo, 123, Cidade', 50)");

            migrationBuilder.Sql(@"
                INSERT INTO ""JOAO"".""camera"" (""status_operacional"", ""localizacao_fisica"", ""id_patio"")
                VALUES ('Ativa', 'Entrada Principal do Pátio', 1)");

            migrationBuilder.Sql(@"
                INSERT INTO ""JOAO"".""moto"" (""placa"", ""modelo"", ""fabricante"", ""ano"", ""id_patio"", ""localizacao_atual"")
                VALUES ('ABC-1234', 'CB 500', 'Honda', 2022, 1, 'Pátio Central')");

            migrationBuilder.Sql(@"
                INSERT INTO ""JOAO"".""aruco_tag"" (""codigo"", ""status"", ""id_moto"")
                VALUES ('TAG-001', 'Ativa', 1)");

            migrationBuilder.Sql(@"
                INSERT INTO ""JOAO"".""localidade"" (""data_hora"", ""ponto_referencia"", ""id_moto"", ""id_patio"", ""id_camera"")
                VALUES (TO_TIMESTAMP('2025-10-28 08:00:00', 'YYYY-MM-DD HH24:MI:SS'), 'Entrada Principal', 1, 1, 1)");

            migrationBuilder.Sql(@"
                INSERT INTO ""JOAO"".""registro_status"" (""tipo_status"", ""descricao"", ""data_status"", ""id_moto"", ""id_funcionario"")
                VALUES ('Entrada', 'Moto chegou ao pátio', TO_TIMESTAMP('2025-10-28 08:15:00', 'YYYY-MM-DD HH24:MI:SS'), 1, 1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aruco_tag");

            migrationBuilder.DropTable(
                name: "localidade");

            migrationBuilder.DropTable(
                name: "registro_status");

            migrationBuilder.DropTable(
                name: "camera");

            migrationBuilder.DropTable(
                name: "funcionario");

            migrationBuilder.DropTable(
                name: "moto");

            migrationBuilder.DropTable(
                name: "patio");
        }
    }
}