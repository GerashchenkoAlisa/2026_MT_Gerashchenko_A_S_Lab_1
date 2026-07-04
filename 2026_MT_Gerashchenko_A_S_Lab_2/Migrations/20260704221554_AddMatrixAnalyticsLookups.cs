using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Migrations
{
    /// <inheritdoc />
    public partial class AddMatrixAnalyticsLookups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    RepositoryPath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ApplicationId);
                });

            migrationBuilder.CreateTable(
                name: "BenchmarkTests",
                columns: table => new
                {
                    BenchmarkTestId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestDescription = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenchmarkTests", x => x.BenchmarkTestId);
                });

            migrationBuilder.CreateTable(
                name: "ErrorCodes",
                columns: table => new
                {
                    ErrorCodeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CodeValue = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CodeDescription = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorCodes", x => x.ErrorCodeId);
                });

            migrationBuilder.CreateTable(
                name: "ExecutionResults",
                columns: table => new
                {
                    ExecutionResultId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ResultName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    ResultDescription = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionResults", x => x.ExecutionResultId);
                });

            migrationBuilder.CreateTable(
                name: "MessageSeverities",
                columns: table => new
                {
                    MessageSeverityId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SeverityName = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    SeverityDescription = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageSeverities", x => x.MessageSeverityId);
                });

            migrationBuilder.CreateTable(
                name: "ProcessorModels",
                columns: table => new
                {
                    ProcessorModelId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProcessorName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PhysicalCores = table.Column<int>(type: "INTEGER", nullable: false),
                    LogicalCores = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessorModels", x => x.ProcessorModelId);
                });

            migrationBuilder.CreateTable(
                name: "ProcessStages",
                columns: table => new
                {
                    ProcessStageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StageName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessStages", x => x.ProcessStageId);
                });

            migrationBuilder.CreateTable(
                name: "SystemEnvironments",
                columns: table => new
                {
                    SystemEnvironmentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EnvironmentName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    EnvironmentDetails = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemEnvironments", x => x.SystemEnvironmentId);
                });

            migrationBuilder.CreateTable(
                name: "BuildExecutions",
                columns: table => new
                {
                    BuildExecutionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProcessStageId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExecutionResultId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExecutionStartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExecutionTimeMs = table.Column<long>(type: "INTEGER", nullable: false),
                    ExitCode = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildExecutions", x => x.BuildExecutionId);
                    table.ForeignKey(
                        name: "FK_BuildExecutions_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildExecutions_ExecutionResults_ExecutionResultId",
                        column: x => x.ExecutionResultId,
                        principalTable: "ExecutionResults",
                        principalColumn: "ExecutionResultId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildExecutions_ProcessStages_ProcessStageId",
                        column: x => x.ProcessStageId,
                        principalTable: "ProcessStages",
                        principalColumn: "ProcessStageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServerConfigurations",
                columns: table => new
                {
                    ServerConfigurationId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProcessorModelId = table.Column<int>(type: "INTEGER", nullable: true),
                    MemoryCapacityGb = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    SystemEnvironmentId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerConfigurations", x => x.ServerConfigurationId);
                    table.ForeignKey(
                        name: "FK_ServerConfigurations_ProcessorModels_ProcessorModelId",
                        column: x => x.ProcessorModelId,
                        principalTable: "ProcessorModels",
                        principalColumn: "ProcessorModelId");
                    table.ForeignKey(
                        name: "FK_ServerConfigurations_SystemEnvironments_SystemEnvironmentId",
                        column: x => x.SystemEnvironmentId,
                        principalTable: "SystemEnvironments",
                        principalColumn: "SystemEnvironmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuildMessages",
                columns: table => new
                {
                    BuildMessageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BuildExecutionId = table.Column<int>(type: "INTEGER", nullable: false),
                    MessageTimestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MessageSeverityId = table.Column<int>(type: "INTEGER", nullable: false),
                    ErrorCodeId = table.Column<int>(type: "INTEGER", nullable: true),
                    MessageText = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildMessages", x => x.BuildMessageId);
                    table.ForeignKey(
                        name: "FK_BuildMessages_BuildExecutions_BuildExecutionId",
                        column: x => x.BuildExecutionId,
                        principalTable: "BuildExecutions",
                        principalColumn: "BuildExecutionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildMessages_ErrorCodes_ErrorCodeId",
                        column: x => x.ErrorCodeId,
                        principalTable: "ErrorCodes",
                        principalColumn: "ErrorCodeId");
                    table.ForeignKey(
                        name: "FK_BuildMessages_MessageSeverities_MessageSeverityId",
                        column: x => x.MessageSeverityId,
                        principalTable: "MessageSeverities",
                        principalColumn: "MessageSeverityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerformanceMetrics",
                columns: table => new
                {
                    PerformanceMetricId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BenchmarkTestId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServerConfigurationId = table.Column<int>(type: "INTEGER", nullable: false),
                    BuildExecutionId = table.Column<int>(type: "INTEGER", nullable: false),
                    SingleThreadTimeMs = table.Column<long>(type: "INTEGER", nullable: false),
                    MultiThreadTimeMs = table.Column<long>(type: "INTEGER", nullable: false),
                    MetricRecordTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceMetrics", x => x.PerformanceMetricId);
                    table.ForeignKey(
                        name: "FK_PerformanceMetrics_BenchmarkTests_BenchmarkTestId",
                        column: x => x.BenchmarkTestId,
                        principalTable: "BenchmarkTests",
                        principalColumn: "BenchmarkTestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerformanceMetrics_BuildExecutions_BuildExecutionId",
                        column: x => x.BuildExecutionId,
                        principalTable: "BuildExecutions",
                        principalColumn: "BuildExecutionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerformanceMetrics_ServerConfigurations_ServerConfigurationId",
                        column: x => x.ServerConfigurationId,
                        principalTable: "ServerConfigurations",
                        principalColumn: "ServerConfigurationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_RepositoryPath",
                table: "Applications",
                column: "RepositoryPath",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BenchmarkTests_TestDescription",
                table: "BenchmarkTests",
                column: "TestDescription",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuildExecutions_ApplicationId",
                table: "BuildExecutions",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildExecutions_ExecutionResultId",
                table: "BuildExecutions",
                column: "ExecutionResultId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildExecutions_ProcessStageId",
                table: "BuildExecutions",
                column: "ProcessStageId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildMessages_BuildExecutionId",
                table: "BuildMessages",
                column: "BuildExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildMessages_ErrorCodeId",
                table: "BuildMessages",
                column: "ErrorCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildMessages_MessageSeverityId",
                table: "BuildMessages",
                column: "MessageSeverityId");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorCodes_CodeValue",
                table: "ErrorCodes",
                column: "CodeValue",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionResults_ResultName",
                table: "ExecutionResults",
                column: "ResultName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageSeverities_SeverityName",
                table: "MessageSeverities",
                column: "SeverityName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceMetrics_BenchmarkTestId",
                table: "PerformanceMetrics",
                column: "BenchmarkTestId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceMetrics_BuildExecutionId",
                table: "PerformanceMetrics",
                column: "BuildExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceMetrics_ServerConfigurationId",
                table: "PerformanceMetrics",
                column: "ServerConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessorModels_ProcessorName",
                table: "ProcessorModels",
                column: "ProcessorName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStages_StageName",
                table: "ProcessStages",
                column: "StageName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServerConfigurations_ProcessorModelId",
                table: "ServerConfigurations",
                column: "ProcessorModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerConfigurations_SystemEnvironmentId",
                table: "ServerConfigurations",
                column: "SystemEnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemEnvironments_EnvironmentName",
                table: "SystemEnvironments",
                column: "EnvironmentName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuildMessages");

            migrationBuilder.DropTable(
                name: "PerformanceMetrics");

            migrationBuilder.DropTable(
                name: "ErrorCodes");

            migrationBuilder.DropTable(
                name: "MessageSeverities");

            migrationBuilder.DropTable(
                name: "BenchmarkTests");

            migrationBuilder.DropTable(
                name: "BuildExecutions");

            migrationBuilder.DropTable(
                name: "ServerConfigurations");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "ExecutionResults");

            migrationBuilder.DropTable(
                name: "ProcessStages");

            migrationBuilder.DropTable(
                name: "ProcessorModels");

            migrationBuilder.DropTable(
                name: "SystemEnvironments");
        }
    }
}
