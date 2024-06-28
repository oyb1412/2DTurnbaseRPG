### 

# TurnbaseRPG


---

## Description

---


- 🔊프로젝트 소개

  TurnbaseRPG는 맵을 탐험하고, 적과 부딪히면 전투 상태로 돌입하는 '포켓몬스터'류의 턴RPG 게임입니다. Tweening을 적극 활용해 실감나는 전투를 구현하는 것에 중점을 두었습니다.

       

- 개발 기간 : 2024.12.29 - 2024.01.05

- 🛠️사용 기술

   -언어 : C#

   -엔진 : Unity Engine

   -데이터베이스 : 로컬

   -개발 환경: Windows 10, Unity 2021.3.10f1



- 💻구동 화면

![스크린샷(5)](https://github.com/oyb1412/2DTurnbaseRPG/assets/154235801/91479e83-83db-4600-9f6a-e60c889f9f54)

## 목차

---

- 기획 의도
- 핵심 로직


### 기획 의도

---

- '턴 시스템'의 구현

- 물리엔진의 도움을 받지않고 전투 시스템 구현

### 핵심 로직

---
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)
### ・턴 시스템을 상태 패턴으로 구현

각 플레이어의 공격 순서를 속도에 맞춰 지정 후, 대기, 공격, 스킬 등 각 행동에 맞춰 턴을 진행하는 시스템 구축

![4](https://github.com/oyb1412/2DTurnbaseRPG/assets/154235801/e9e8e54f-212d-4ef5-bd35-cd7263a43071)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)


### ・다형성을 활용한 플레이어 턴 순서 지정

모든 유닛의 공격 속도를 비교해 플레이어의 턴 순서를 정하기 위해, 플레이어 유닛/ 적 유닛을 동일한 객체로 취급.

![3](https://github.com/oyb1412/2DTurnbaseRPG/assets/154235801/007e4918-9d89-4339-903a-5956c76ad71f)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)

### ・타일맵 충돌 판정

산, 나무, 강 등 다양한 형태의 타일맵들과 플레이어간의 충돌 판정 구현

![1](https://github.com/oyb1412/2DTurnbaseRPG/assets/154235801/bc7a343d-9500-4063-9c14-1be935bdfbeb)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)
### ・물리엔진을 사용하지 않은 공격 및 피격 판정

퍼포먼스 상승을 위해 물리엔진을 사용하지 않고 충돌판정 구현

![2](https://github.com/oyb1412/2DTurnbaseRPG/assets/154235801/5df230f8-fc1c-44f3-906c-12339f4dc988)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)

