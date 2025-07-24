# 🧑‍🌾 Farm Alive - VR 직업 체험 프로젝트 (기업협약 합반 팀프로젝트)

---

## 📌 프로젝트 개요

- 미래 직업을 VR 환경에서 몰입감 있게 체험할 수 있도록 설계한 팀 프로젝트입니다.
- Firebase를 기반으로 사용자 데이터를 실시간 저장하고, Fusion+PUN 네트워크 구조를 활용해 로비와 인게임을 분리 설계했습니다.
- IK 시스템을 통해 VR 컨트롤러의 움직임을 자연스럽게 반영하며, 데이터 최적화와 네트워크 안정성을 고려한 구조로 완성했습니다.

---

## 🛠️ 개발 환경

- 개발기간: 2024.12.12 ~ 2025.01.22
- 개발인원: 개발 5명 + 기획 5명
- 사용툴: Unity 2021.3.42f1, Visual Studio 2022, GitHub, Notion

---

## 🎥 시연 영상

[시연 영상 링크](https://youtu.be/euzIAoj1ujY)

---

## 🔗 링크 모음

- [PPT 자료](https://docs.google.com/presentation/d/1CSAigM28Yav0pndeEJ-jUXS--VRhtH5y/edit?usp=drive_link&ouid=110574879283798050846&rtpof=true&sd=true)
- [GitHub 코드](https://github.com/Geniards/Farm-Alive/tree/main/Assets/Develop/KMS/Scripts)
- [Notion 문서](https://www.notion.so/1-5-19728bcaf90480868cc7ea004560826c?pvs=21)
- [기술문서](https://www.notion.so/Farm-Alive-1ca28bcaf90480d98115d059c305eb3f)

---

## 🧩 핵심 기능 요약

### 1. Firebase 사용자 인증 및 데이터 저장
- VR 환경에서 키보드 입력 최소화를 위해 **익명 로그인** 방식 채택
- UID 기반 닉네임, 스테이지, 진행 정보 실시간 저장

### 2. 로컬 캐싱 최적화
- Firebase 데이터 로컬 캐싱 후 동기화 방식으로 **네트워크 의존도 최소화**

### 3. Fusion + PUN 네트워크 분리 설계
- **Fusion**: 로비에서 자유 이동, 유저 간 상호작용 동기화
- **PUN**: 인게임 전 매치메이킹 및 대기방 구현

### 4. VR IK 기반 캐릭터 리깅
- **손/팔은 컨트롤러 기반 IK 적용**, **하체는 Animator로 이동 처리**
- **Layer 마스킹**으로 본인 시점에서 몸체 비노출 처리

---
