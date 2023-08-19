# 스네이크 게임

<p align="center">
  <img width="460" height="300" src="https://github.com/riviea/Snake-Game/assets/12423098/8fcf8a23-7e3e-46cf-9012-18ac659c9fda">
</p>

- 방향키를 눌러서 뱀(◆)을 조작합니다.
- 자신의 몸통, 혹은 외벽(■)과 부딪히면 게임오버됩니다.
- 음식(◎)을 먹으면 게임의 속도가 빨라집니다.
- 게임오버 시 프로그램이 종료됩니다.


![image](https://github.com/riviea/Snake-Game/assets/12423098/a381c3cd-bdf6-46d2-9fd6-e5d4c81f8276)

- 위 사진과 같이 구조를 설계했습니다.
- `Shape` 객체를 상속하여 뱀의 몸통인 `Cell`, 맵의 `Tile` 그리고 음식 `Food` 객체를 정의했습니다.
- 해당 객체를 여러 개 사용하는 `Snake`, `Map`, `FoodCreator` 객체를 정의해 게임에 필요한 기능을 작성했습니다.
- 해당 객체를 관리하는 `Game` 객체를 만들어서 게임 진행에 필요한 `Update`, `Render` 로직을 처리했습니다.
