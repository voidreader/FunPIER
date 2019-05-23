@echo off

GenTextTable kysp_database.xlsx
move kysp_database.bytes kysp_text.bytes
copy kysp_text.bytes C:\Git\PierCorp-Unity\KYSP_EN\Assets\Resources\Others\kysp_text.bytes

GenStageInfo kysp_database.xlsx
move kysp_database.bytes kysp_stage.bytes
copy kysp_stage.bytes C:\Git\PierCorp-Unity\KYSP_EN\Assets\Resources\Others\kysp_stage.bytes

GenDesignParams kysp_database.xlsx
move kysp_database.bytes kysp_params.bytes
copy kysp_params.bytes C:\Git\PierCorp-Unity\KYSP_EN\Assets\Resources\Others\kysp_params.bytes