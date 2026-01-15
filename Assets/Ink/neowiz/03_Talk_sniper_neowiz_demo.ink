VAR DEBUG = false

VAR trust_sniper = 0
VAR first_meet_check = false

저 앞에 저격병이 보인다. 말을 걸어볼까?
+ [좋아]
    말을 걸어봐야 겠다. 
    {first_meet_check: ->sniper_talk_start | ->sniper_talk_first}
    
+ [아니]
    나중에 이야기하자.->END

===sniper_talk_first===
~first_meet_check = true

- 아 씨-발 뭐야?!
- 뭐야,<br>이번에 새로온 함장아니야?
- 
->sniper_talk_topic_select

===sniper_talk_start===
{DEBUG:
    (DEBUG) trust_sniper = {trust_sniper}
}

{
- trust_sniper < 30:
{~뭐냐. |무슨 일이지?}

- trust_sniper >= 30 && trust_sniper < 50:
{~왔나. |느려터졌군.}

- trust_sniper >= 50:
{~이제 오는거냐?|반갑다.}
} 

+ [저격병과 대화를 시작한다.]
    ->sniper_talk_topic_select
+ [떠난다.]
    ->END

===sniper_talk_topic_select===
- 할 말 있어?

* [이전 함장은 어땠어?]
    ->sniper_talk_topic_1
* [제일 기억에 남는 임무는 뭐였어?]
    ->sniper_talk_topic_2
* [특기는 뭐야?]
    ->sniper_talk_topic_3
+ ->
    (이제 다른 사람과 대화 해보자.)
    ->END
    
===sniper_talk_topic_1===
// 이전 함장을 주제로 나누는 대화 내용
- 이전 함장..? 
- 씨발 니가 그걸 알아서 뭘 할꺼냐.
->sniper_talk_topic_select

===sniper_talk_topic_2===
// 기억에 남는 임무를 주제로 나누는 대화 내용
- 제일 기억에 남는 임무라...
- ...
- 이전 함장과 같이 했던 마지막 임무가 기억에 제일 남는다.
->sniper_talk_topic_select

===sniper_talk_topic_3===
// 특기를 주제로 나누는 대화 내용
- 내 특기는 저격이다.
- 어떤 종족이든 내 총알에 맞으면 즉사지.
->sniper_talk_topic_select
