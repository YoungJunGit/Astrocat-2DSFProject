VAR DEBUG = false
VAR trust_commissar = 0
VAR first_meet_check = false

저 앞에 커미사르가 보인다. 말을 걸어볼까?

+ [좋아]
    말을 걸어봐야 겠다.
    { first_meet_check:
        -> commissar_talk_start
    - else:
        -> commissar_talk_first
    }

+ [아니]
    나중에 이야기하자.
    -> END


=== commissar_talk_first ===
~ first_meet_check = true

아, 자네가 새로운 함장이군.
반갑네.

* 그가 손을 내밀어 악수를 청한다.

우주신의 가호가 자네와 함께 할걸세.
부디 자네는 오래동안 함께하길 바라네.
아. 방금 말은 신경쓰지 말게나.
내 입이 방정이군.
-> commissar_talk_topic_select


=== commissar_talk_start ===
{DEBUG:
(DEBUG) trust_commissar = {trust_commissar}
}

{ 
- trust_commissar < 30:
    {~ 반갑군. | 좋은 아침일세. }
- trust_commissar >= 30 && trust_commissar < 50:
    {~ 함장, 오늘 저녁은 뭔가? | 같이 기도하러 가겠나? }
}

+ [커미사르와 대화를 시작한다.]
    -> commissar_talk_topic_select

+ [떠난다.]
    -> END


=== commissar_talk_topic_select ===
궁금한게 있나?

* [우주신에 대하여.]
    -> commissar_talk_topic_1

* [제일 기억에 남는 임무는 뭐였어?]
    -> commissar_talk_topic_2

* [특기는 뭐야?]
    -> commissar_talk_topic_3

+ ->
    (이제 다른 사람과 대화 해보자.)
    -> END


=== commissar_talk_topic_1 ===
외계 종족과 만나기 전까지 지구에는 많은 신들이 있었지.
외계 종족과 교류하며 외계 종족에게도 종교가 있다는게 밝혀졌네.
그 이후로 다양한 종교들에 대한 심도있는 연구가 시작됐네.
연구는 아직 진행중이지만,
우주신이 이 세상을 창조한 유일무이한 신이라는 것이 현재의 정설이지.
나는 현재 우주신 종교의 사제 신분으로 UGPD 본부에서 이 함선에 파견되었다네.
자네도 종교에 관심이 있나?

+ [없다고 둘러댄다.]
    유감이군.
    -> commissar_talk_topic_select


=== commissar_talk_topic_2 ===
아무래도 이단을 광적으로 믿는 외계 종족이 기억에 남네.
-> commissar_talk_topic_select


=== commissar_talk_topic_3 ===
내 특기라..
아군에게 힘을 복돋아 주고, 체력을 회복시켜줄 수 있다네.
-> commissar_talk_topic_select
