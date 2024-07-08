## 9 号大致安排

- 完成开发环境搭建，并跑通最小示例；
- 确定项目整体的风格设计，包括：
    - 界面大体风格；
    - API 功能划分与设计风格；
    - 控制器功能划分。
- 项目正式开工。

## 注意事项

1. 不要把数据库密码直接放到 GitHub 上，appsettings.json 文件会放在群里；
2. 不要使用 sys 用户完成业务逻辑；
3. 注意代码规范，推荐使用 VS 的格式化工具（`Ctrl-K Ctrl-D`）规整代码；
4. 注意 git 提交信息的规范（动作加修改简述，可选动作参考 git 管理部分）。

## 工作流程

开发 - 测试 - 发布

### git 管理

- 本次项目采用的 git 模型主要使用 main dev feature 三类分支
- 各组平时可在各自 feature 分支内任意修改
- 部分功能完成后可以在 GitHub 发起合并申请（Merge Request），代码评审后合并进 dev 分支
- dev 分支稳定后合并到 main 分支并发布

git 的 commit 信息中可以填写如下动作，不必写提交影响的模块或文件：
- feat : 新功能
- fix : 修复bug
- docs : 文档改变
- style : 代码格式改变
- refactor : 某个已有功能重构
- perf : 性能优化
- test : 增加或更正测试
- build : 改变了 build 工具 如 grunt 换成了 npm
- revert : 撤销上一次的 commit
- chore : 构建过程或辅助工具的变动

### 开发要求

开发调试使用 swagger，写好 API 文档
后端编译运行指定使用 VS

## 当前进度

- ADMINISTRATOR 中的数据暂时是通过开发工具添加的；
- 前端与后端、后端与数据库的交互已验证可行。

## 设计参考

后端采用 MVC 架构，其中 Model 主要参考之前完成的数据库设计，View 由前端实现，即后端主要解决 Controller 的逻辑实现。

API 设计参考 RESTful 设计方法：
- 资源（Resources）：如本次的商品、用户；
- 表现层（Representation）：资源的表现形式，如 JSON，前端操作资源的主要途径；
- 状态转移（State Transfer）：前后端之间的交互无状态，每次请求都包含所有必要信息；
- 统一接口（Uniform Interface）：。

## 计划功能

- 基本功能；
- 加入 JWT 身份验证；
- 接入支付接口；
- 推送功能。

