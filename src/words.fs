variable base 10 base !

code 0
	lda	#0
	tay
	jmp	%pushya%
;code
1 constant 1

: chars ;
: char+ 1+ ;
: align ;
: aligned ;

: negate invert 1+ ;
: if postpone 0branch here 0 , ; immediate
: begin here ; immediate

variable end
create hold-buffer 34 allot
: <# hold-buffer end ! ;
: #> 2drop hold-buffer end @ over - ;
: hold
hold-buffer dup 1+ end @ hold-buffer - move
1 end +!  hold-buffer c! ;
: sign 0< if '-' hold then ;
: ud/mod
>r 0 r@ um/mod r> swap >r um/mod r> ;
: # base @ ud/mod rot
dup $a < if 7 - then $37 + hold ;
: #s # begin 2dup or while # repeat ;

: i postpone r@ ; immediate
: nip swap drop ;
: \ refill 0= if source nip >in ! then ; immediate
: 2r@ r> r> r> 2dup >r >r rot rot swap >r ;
: 2>r r> rot rot swap >r >r >r ;
: 2r> r> r> r> swap rot >r ;
: u> swap u< ;
: 2+ 1+ 1+ ;
: cell+ 2+ ;
: 2@ dup 2+ @ swap @ ;
: 2! swap over ! 2+ ! ;
: cells 2* ;
: s>d dup 0< ;
: min 2dup < if drop else nip then ;
: max 2dup > if drop else nip then ;
: ?dup dup if dup then ;
: case 0 ; immediate
: endcase postpone drop begin ?dup while postpone then repeat ; immediate
: of postpone (of) here 0 , ; immediate
: endof postpone else ; immediate
: value create , does> @ ; \ TODO Optimized VALUE/TO, like DurexForth.
: 0<> 0= 0= ;
: 0> dup 0< 0= swap 0<> and ;
: <> = 0= ;
: buffer: create allot ;
: hex $10 base ! ;
: decimal #10 base ! ;
: true -1 ;
: false 0 ;
: bl $20 ;
: space bl emit ;
: . s>d swap over dabs <# #s rot sign #> type space ;
: u. 0 <# #s #> type space ;
: save-input >in @ 1 ;
: restore-input drop >in ! 0 ;
: spaces begin dup 0> while space 1- repeat drop ;
: .s ." <" depth s>d swap over dabs <# #s rot sign #> type ." > "
depth 1+ 1 ?do depth i - pick . loop cr ;
: .r ( n1 n2 -- )
swap s>d swap over dabs <# #s rot sign #>
rot over - spaces type space ;
: u.r ( u n -- )
swap 0 <# #s #> rot over - spaces type space ;
create pad 84 allot
: erase 0 fill ;
: 2over 3 pick 3 pick ;
: 2swap >r rot rot r> rot rot ;
: [ 0 state ! ; immediate
: ] -1 state ! ;
: count dup 1+ swap c@ ;
: /string dup >r - swap r> + swap ;
: abort depth 0 ?do drop loop quit ;
: abort" postpone if postpone ." postpone cr postpone abort postpone then ; immediate
: within over - >r - r> u< ; \ forth-standard.org
: roll ?dup if swap >r 1- recurse r> swap then ;

\ from test suite
: S=  \ ( ADDR1 C1 ADDR2 C2 -- T/F ) COMPARE TWO STRINGS.
   >R SWAP R@ = IF         \ MAKE SURE STRINGS HAVE SAME LENGTH
      R> ?DUP IF         \ IF NON-EMPTY STRINGS
    0 DO
       OVER C@ OVER C@ - IF 2DROP FALSE UNLOOP EXIT THEN
       SWAP CHAR+ SWAP CHAR+
         LOOP
      THEN
      2DROP TRUE         \ IF WE GET HERE, STRINGS MATCH
   ELSE
      R> DROP 2DROP FALSE      \ LENGTHS MISMATCH
   THEN ;

: m+ s>d d+ ;
: dnegate invert >r invert r> 1 m+ ;

: fm/mod \ from Gforth
dup >r dup 0< if negate >r dnegate r> then
over 0< if tuck + swap then um/mod
r> 0< if swap negate swap then ;

( from FIG UK )
: /mod >r s>d r> fm/mod ;
: / /mod nip ;
: mod /mod drop ;
: */mod >r m* r> fm/mod ;
: */ */mod nip ;
: ?negate 0< if negate then ;
: sm/rem 2dup xor >r over >r abs >r dabs r> um/mod swap r> ?negate swap r> ?negate ;

( from forth-standard.org )
: isspace? BL 1+ U< ;
: isnotspace? isspace? 0= ;
: xt-skip >R BEGIN DUP WHILE OVER C@ R@ EXECUTE WHILE 1 /STRING REPEAT THEN R> DROP ;
: parse-name SOURCE >IN @ /STRING ['] isspace? xt-skip OVER >R ['] isnotspace? xt-skip 2DUP 1 MIN + SOURCE DROP - >IN ! DROP R> TUCK - ;
: DEFER CREATE ['] ABORT , DOES> @ EXECUTE ;
: defer! >body ! ;
: defer@ >body @ ;
: ACTION-OF STATE @ IF POSTPONE ['] POSTPONE DEFER@ ELSE ' DEFER@ THEN ; IMMEDIATE
: IS STATE @ IF POSTPONE ['] POSTPONE DEFER! ELSE ' DEFER! THEN ; IMMEDIATE
: HOLDS BEGIN DUP WHILE 1- 2DUP + C@ HOLD REPEAT 2DROP ;

code	d+	; ( d1 d2 -- d3 )
	clc
	lda	LSB+1,x
	adc	LSB+3,x
	sta	LSB+3,x
	lda	MSB+1,x
	adc	MSB+3,x
	sta	MSB+3,x
	lda	LSB,x
	adc	LSB+2,x
	sta	LSB+2,x
	lda	MSB,x
	adc	MSB+2,x
	sta	MSB+2,x
	inx
	inx
	rts
;code

: accumulate ( +d0 addr digit - +d1 addr )
swap >r swap base @ um* drop
rot base @ um* d+ r> ;
: pet# ( char -- num )
$7f and dup \ lowercase
':' < if '0' else '7' then - ;
: digit? ( char -- flag )
pet# dup 0< 0= swap base @ < and ;
: >number ( ud addr u -- ud addr u )
begin dup 0= if exit then
over c@ digit? while
>r dup c@ pet# accumulate
1+ r> 1- repeat ;

\ ----- C64 primitives below

code	c@
	lda	LSB,x
	sta	+ + 1
	lda	MSB,x
	sta	+ + 2
+	lda	$cafe
	sta	LSB,x
	lda	#0
	sta	MSB,x
	rts
;code

code c!
	lda	LSB,x
	sta	+ + 1
	lda	MSB,x
	sta	+ + 2
	lda	LSB+1,x
+	sta	$1234
	inx
	inx
	rts
;code

code 1+
	inc LSB, x
	bne +
	inc MSB, x
+	rts
;code

code litc
	dex

	; load IP
	pla
	sta W
	pla
	sta W + 1

	inc W
	bne +
	inc W + 1
+
	; copy literal to stack
	ldy #0
	lda (W), y
	sta LSB, x
	sty MSB, x

	inc W
	bne +
	inc W + 1
+	jmp (W)
;code

code lit
	dex

	; load IP
	pla
	sta W
	pla
	sta W + 1

	; copy literal to stack
	ldy #1
	lda (W), y
	sta LSB, x
	iny
	lda (W), y
	sta MSB, x

	lda W
	clc
	adc #3
	sta + + 1
	lda W + 1
	adc #0
	sta + + 2
+	jmp $1234
;code

code (loop)
	stx	W	; x = stack pointer
	tsx

	inc	$103,x	; i++
	bne	+
	inc	$104,x
+
	lda	$103,x	; lsb check
	cmp	$105,x
	beq	.check_msb

.continue_loop
	ldx	W	; restore x
	jmp	%branch%

.check_msb
	lda	$104,x
	cmp	$106,x
	bne	.continue_loop

	pla		; loop done - skip branch address
	clc
	adc	#3
	sta	W2

	pla
	adc	#0
	sta	W2 + 1

	txa		; sp += 6
	clc
	adc	#6
	tax
	txs

	ldx	W	; restore x
	jmp	(W2)
;code

code 0branch
	inx
	lda	LSB-1, x
	ora	MSB-1, x
	bne	+
	jmp	%branch%
+ 	; skip offset
	pla
	clc
	adc	#2
	bcc	+
	tay
	pla
	adc	#0
	pha
	tya
+	pha
	rts
;code

code !
	lda LSB, x
	sta W
	lda MSB, x
	sta W + 1

	ldy #0
	lda LSB+1, x
	sta (W), y
	iny
	lda MSB+1, x
	sta (W), y

	inx
	inx
	rts
;code

code 0<
	lda	MSB,x
	and	#$80
	beq	+
	lda	#$ff
+	sta	LSB,x
	sta	MSB,x
	rts
;code

code dup
	dex
	lda MSB + 1, x
	sta MSB, x
	lda LSB + 1, x
	sta LSB, x
	rts
;code

code type
-	lda LSB,x
	ora MSB,x
	bne +
	inx
	inx
	rts
+	jsr %over%
	jsr %c@%
	jsr %emit%
	jsr %1%
	jsr %/string%
	jmp -
;code

code depth
	txa
	eor #$ff
	tay
	iny
	dex
	sty LSB,x
	lda #0
	sta MSB,x
	rts
;code

code @
	lda LSB,x
	sta W
	lda MSB,x
	sta W+1

	ldy #0
	lda (W),y
	sta LSB,x
	iny
	lda (W),y
	sta MSB,x
	rts
;code

code =
	ldy #0
	lda LSB, x
	cmp LSB + 1, x
	bne +
	lda MSB, x
	cmp MSB + 1, x
	bne +
	dey
+	inx
	sty MSB, x
	sty LSB, x
	rts
;code

code (do)
	pla
	sta	W
	pla
	tay

	lda	MSB+1,x
	pha
	lda	LSB+1,x
	pha

	lda	MSB,x
	pha
	lda	LSB,x
	pha

	inx
	inx

	tya
	pha
	lda	W
	pha
	rts
;code

code	j
	txa
	tsx
	ldy	$107,x
	sty	W
	ldy	$108,x
	tax
	dex
	sty	MSB,x
	lda	W
	sta	LSB,x
	rts
;code

code +
	lda LSB, x
	clc
	adc LSB + 1, x
	sta LSB + 1, x

	lda MSB, x
	adc MSB + 1, x
	sta MSB + 1, x

	inx
	rts
;code

code 0=
	ldy #0
	lda MSB, x
	bne +
	lda LSB, x
	bne +
	dey
+	sty MSB, x
	sty LSB, x
	rts
;code

code sliteral
	jsr	%r>%
	jsr	%1+%
	jsr	%dup%
	jsr	%1+%
	jsr	%swap%
	jsr	%c@%
	jsr	%2dup%
	jsr	%+%
	jsr	%1-%
	jsr	%>r%
	rts
;code

code 1-
	lda LSB, x
	bne +
	dec MSB, x
+	dec LSB, x
	rts
;code

code 2dup
	jsr %over%
	jmp %over%
;code

code over
	dex
	lda MSB + 2, x
	sta MSB, x
	lda LSB + 2, x
	sta LSB, x
	rts
;code

code swap
	ldy MSB, x
	lda MSB + 1, x
	sta MSB, x
	sty MSB + 1, x

	ldy LSB, x
	lda LSB + 1, x
	sta LSB, x
	sty LSB + 1, x
	rts
;code

code cr
	jsr	%litc%
	!byte	$d
	jmp	%emit%
;code

code emit
	lda	LSB, x
	inx
	jmp	PUTCHR
;code

code /string
	jsr %dup%
	jsr %>r%
	jsr %-%
	jsr %swap%
	jsr %r>%
	jsr %+%
	jmp %swap%
;code

code -
	lda LSB + 1, x
	sec
	sbc LSB, x
	sta LSB + 1, x
	lda MSB + 1, x
	sbc MSB, x
	sta MSB + 1, x
	inx
	rts
;code

code pushya
	dex
	sta	LSB, x
	sty	MSB, x
	rts
;code

code invert
	lda MSB, x
	eor #$ff
	sta MSB, x
	lda LSB, x
	eor #$ff
	sta LSB,x
	rts
;code

code branch
	pla
	sta W
	pla
	sta W + 1

	ldy #2
	lda (W), y
	sta + + 2
	dey
	lda (W), y
	sta + + 1
+	jmp $1234
;code

code	dabs
	jsr	%dup%
	jmp	%?dnegate%
;code

code	?dnegate
	jsr	%0<%
	inx
	lda	MSB-1,x
	beq	+
	jsr	%dnegate%
+	rts
;code

code	+!
	lda	LSB,x
	sta	W
	lda	MSB,x
	sta	W+1

	ldy	#0
	clc

	lda	(W),y
	adc	LSB+1,x
	sta	(W),y
	iny
	lda	(W),y
	adc	MSB+1,x
	sta	(W),y
	inx
	inx
	rts
;code

code	2*
	asl	LSB, x
	rol	MSB, x
	rts
;code

code	2/
	lda	MSB,x
	cmp	#$80
	ror	MSB,x
	ror	LSB,x
	rts
;code

code	and
	lda	MSB, x
	and	MSB + 1, x
	sta	MSB + 1, x

	lda	LSB, x
	and	LSB + 1, x
	sta	LSB + 1, x

	inx
	rts
;code

code	r>	; must be called using jsr
	pla
	sta W
	pla
	sta W+1
	inc W
	bne +
	inc W+1
+
	dex
	pla
	sta LSB,x
	pla
	sta MSB,x
	jmp (W)
;code

code	r@	; must be called using jsr
	txa
	tsx
	ldy $103,x
	sty W
	ldy $104,x
	tax
	dex
	sty MSB,x
	lda W
	sta LSB,x
	rts
;code

code	>r	; must be called using jsr
	pla
	sta W
	pla
	sta W+1
	inc W
	bne +
	inc W+1
+
	lda MSB,x
	pha
	lda LSB,x
	pha
	inx
	jmp (W)
;code

code	or
	lda	MSB,x
	ora	MSB+1,x
	sta	MSB+1,x
	lda	LSB,x
	ora	LSB+1,x
	sta	LSB+1,x
	inx
	rts
;code

code	xor
	lda	MSB,x
	eor	MSB+1,x
	sta	MSB+1,x
	lda	LSB,x
	eor	LSB+1,x
	sta	LSB+1,x
	inx
	rts
;code

code	lshift
-	dec	LSB,x
	bmi	+
	asl	LSB+1,x
	rol	MSB+1,x
	jmp	-
+	inx
	rts
;code

code	rshift
-	dec	LSB,x
	bmi	+
	lsr	MSB+1,x
	ror	LSB+1,x
	jmp	-
+	inx
	rts
;code

code	<
    ldy #0
    sec
    lda LSB+1,x
    sbc LSB,x
    lda MSB+1,x
    sbc MSB,x
    bvc +
    eor #$80
+   bpl +
    dey
+   inx
    sty LSB,x
    sty MSB,x
    rts
;code

code	>
	jsr	%swap%
	jmp	%<%
;code

code	u<
    ldy #0
    lda MSB, x
    cmp MSB + 1, x
    bcc .false
    bne .true
    ; ok, msb are equal...
    lda LSB + 1, x
    cmp LSB, x
    bcs .false
.true
    dey
.false
    inx
    sty MSB, x
    sty LSB, x
    rts
;code

code	pick
    txa
    sta + + 1
    clc
    adc LSB,x
    tax
    inx
    lda LSB,x
    ldy MSB,x
+   ldx #0
    sta LSB,x
    sty MSB,x
    rts
;code

code	rot
	ldy	MSB+2,x
	lda	MSB+1,x
	sta	MSB+2,x
	lda	MSB,x
	sta	MSB+1,x
	sty	MSB,x
	ldy	LSB+2,x
	lda	LSB+1,x
	sta	LSB+2,x
	lda	LSB,x
	sta	LSB+1,x
	sty	LSB,x
	rts
;code

code	abs
	lda	MSB,x
	bpl	+
	jmp	%negate%
+	rts
;code

code	m*
	jsr	%2dup%
	jsr	%xor%
	jsr	%>r%
	jsr	%>r%
	jsr	%abs%
	jsr	%r>%
	jsr	%abs%
	jsr	%um*%
	jsr	%r>%
	jmp	%?dnegate%
;code

code	um*	; wastes W, W2, y
product = W
    lda #$00
    sta product+2 ; clear upper bits of product
    sta product+3
    ldy #$10 ; set binary count to 16
.shift_r
    lsr MSB + 1, x ; multiplier+1 ; divide multiplier by 2
    ror LSB + 1, x ; multiplier
    bcc rotate_r
    lda product+2 ; get upper half of product and add multiplicand
    clc
    adc LSB, x ; multiplicand
    sta product+2
    lda product+3
    adc MSB, x ; multiplicand+1
rotate_r
    ror ; rotate partial product
    sta product+3
    ror product+2
    ror product+1
    ror product
    dey
    bne .shift_r

    lda product
    sta LSB + 1, x
    lda product + 1
    sta MSB + 1, x
    lda product + 2
    sta LSB, x
    lda product + 3
    sta MSB, x
    rts
;code

code	*
	jsr	%m*%
	inx
	rts
;code

code	um/mod
        N = W
        SEC
        LDA     LSB+1,X     ; Subtract hi cell of dividend by
        SBC     LSB,X     ; divisor to see if there's an overflow condition.
        LDA     MSB+1,X
        SBC     MSB,X
        BCS     oflo    ; Branch if /0 or overflow.

        LDA     #17     ; Loop 17x.
        STA     N       ; Use N for loop counter.
loop:   ROL     LSB+2,X     ; Rotate dividend lo cell left one bit.
        ROL     MSB+2,X
        DEC     N       ; Decrement loop counter.
        BEQ     end     ; If we're done, then branch to end.
        ROL     LSB+1,X     ; Otherwise rotate dividend hi cell left one bit.
        ROL     MSB+1,X
        lda     #0
        sta     N+1
        ROL     N+1     ; Rotate the bit carried out of above into N+1.

        SEC
        LDA     LSB+1,X     ; Subtract dividend hi cell minus divisor.
        SBC     LSB,X
        STA     N+2     ; Put result temporarily in N+2 (lo byte)
        LDA     MSB+1,X
        SBC     MSB,X
        TAY             ; and Y (hi byte).
        LDA     N+1     ; Remember now to bring in the bit carried out above.
        SBC     #0
        BCC     loop

        LDA     N+2     ; If that didn't cause a borrow,
        STA     LSB+1,X     ; make the result from above to
        STY     MSB+1,X     ; be the new dividend hi cell
        bcs     loop    ; and then branch up.

oflo:   LDA     #$FF    ; If overflow or /0 condition found,
        STA     LSB+1,X     ; just put FFFF in both the remainder
        STA     MSB+1,X
        STA     LSB+2,X     ; and the quotient.
        STA     MSB+2,X

end:    INX
        jmp %swap%
;code

code	tuck
	jsr	%swap%
	jmp	%over%
;code

code	bye
	jmp	BYE
;code

code	execute
	lda	LSB, x
	sta	W
	lda	MSB, x
	sta	W + 1
	inx
	jmp	(W)
;code

code	(+loop)
	; r> swap r> 2dup +
	jsr	%r>%
	jsr	%swap%
	jsr	%r>%
	jsr	%2dup%
	jsr	%+%

	; rot 0< if tuck swap else tuck then
	jsr	%rot%
	inx
	lda	MSB-1,x
	bpl	.pl
	jsr	%tuck%
	jsr	%swap%
	jmp	++
.pl	jsr	%tuck%
++
	; r@ 1- -rot within 0= if
	jsr	%r@%
	jsr	%1-%
	jsr	%rot%
	jsr	%rot%
	jsr	%within%

	inx
	lda	MSB-1,x
	bne	+

	; >r >r [ ' branch jmp, ] then
	jsr	%>r%
	jsr	%>r%
	jmp	%branch%
+
	; r> 2drop 2+ >r ;
	jsr	%r>%
	inx
	inx
	jsr	%2+%
	jsr	%>r%
	rts
;code

code	dodoes
    ; behavior pointer address => W
    pla
    sta W
    pla
    sta W + 1

    inc W
    bne +
    inc W + 1
+

    ; push data pointer to param stack
    dex
    lda W
    clc
    adc #2
    sta LSB,x
    lda W + 1
    adc #0
    sta MSB,x

    ldy #0
    lda (W),y
    sta W2
    iny
    lda (W),y
    sta W2 + 1
    jmp (W2)
;code

\ from cc65 memmove.s
\ 2003-08-20, Ullrich von Bassewitz
\ 2009-09-13, Christian Krueger -- performance increase (about 20%), 2013-07-25 improved unrolling
\ 2015-10-23, Greg King
code	move
; Check for the copy direction. If dest < src, we must copy upwards (start at
; low addresses and increase pointers), otherwise we must copy downwards
; (start at high addresses and decrease pointers).
ptr1 = W
ptr2 = W2
ptr3 = W3
	txa
	pha

	ldy	#0

	; ptr3 = n
	lda	MSB,x
	sta	ptr3+1
	lda	LSB,x
	sta	ptr3

	; ptr1 = src
	lda	MSB+2,x
	sta	ptr1+1
	lda	LSB+2,x
	sta	ptr1

	; ptr2 = dst
	lda	MSB+1,x
	sta	ptr2+1
	lda	LSB+1,x
	sta	ptr2

; Check for the copy direction. If dest < src, we must copy upwards (start at
; low addresses and increase pointers), otherwise we must copy downwards
; (start at high addresses and decrease pointers).

        cmp     ptr1
	lda	ptr2+1
        sbc     ptr1+1
        bcc     memcpy_upwards  ; Branch if dest < src (upwards copy)

; Copy downwards. Adjust the pointers to the end of the memory regions.

        lda     ptr1+1
	clc
        adc     ptr3+1
        sta     ptr1+1

        lda     ptr2+1
	clc
        adc     ptr3+1
        sta     ptr2+1

; handle fractions of a page size first

        ldy     ptr3            ; count, low byte
        bne     .entry          ; something to copy?
        beq     PageSizeCopy    ; here like bra...

.copyByte:
        lda     (ptr1),y
        sta     (ptr2),y
.entry:
        dey
        bne     .copyByte
        lda     (ptr1),y        ; copy remaining byte
        sta     (ptr2),y

PageSizeCopy:                   ; assert Y = 0
        ldx     ptr3+1          ; number of pages
        beq     done            ; none? -> done

.initBase:
        dec     ptr1+1          ; adjust base...
        dec     ptr2+1
        dey                     ; in entry case: 0 -> FF
.copyBytes:
        lda     (ptr1),y        ; important: unrolling three times gives a nice
        sta     (ptr2),y        ; 255/3 = 85 loop which ends at 0
        dey
        lda     (ptr1),y        ; important: unrolling three times gives a nice
        sta     (ptr2),y        ; 255/3 = 85 loop which ends at 0
        dey
        lda     (ptr1),y        ; important: unrolling three times gives a nice
        sta     (ptr2),y        ; 255/3 = 85 loop which ends at 0
        dey
.copyEntry:                     ; in entry case: 0 -> FF
        bne     .copyBytes
        lda     (ptr1),y        ; Y = 0, copy last byte
        sta     (ptr2),y
        dex                     ; one page to copy less
        bne     .initBase       ; still a page to copy?

done
	pla
	tax
	inx
	inx
	inx
	rts

memcpy_upwards:                 ; assert Y = 0
        ldx     ptr3+1          ; Get high byte of n
        beq     L2              ; Jump if zero

L1:
        lda     (ptr1),Y        ; copy a byte
        sta     (ptr2),Y
        iny
        lda     (ptr1),Y        ; copy a byte
        sta     (ptr2),Y
        iny
        bne     L1
        inc     ptr1+1
        inc     ptr2+1
        dex                     ; Next 256 byte block
        bne     L1              ; Repeat if any

        ; the following section could be 10% faster if we were able to copy
        ; back to front - unfortunately we are forced to copy strict from
        ; low to high since this function is also used for
        ; memmove and blocks could be overlapping!
L2:                             ; assert Y = 0
        ldx     ptr3            ; Get the low byte of n
        beq     done            ; something to copy

L3:     lda     (ptr1),Y        ; copy a byte
        sta     (ptr2),Y
        iny
        dex
        bne     L3
	jmp	done
;code

\ from cc65 memset.s
\ Ullrich von Bassewitz, 29.05.1998
\ Performance increase (about 20%) by
\ Christian Krueger, 12.09.2009, slightly improved 12.01.2011
code	fill
ptr1 = W
ptr2 = W2
ptr3 = W3

	txa
	pha

	lda	MSB+1,x
	sta	ptr3+1
	lda	LSB+1,x
        sta     ptr3            ; Save n

	; ptr1 = c-addr
	lda	MSB+2,x
	sta	ptr1+1
	lda	LSB+2,x
	sta	ptr1

	; x = char
	lda	LSB,x
        tax
	ldy	#0

        lsr     ptr3+1          ; divide number of
        ror     ptr3            ; bytes by two to increase
        bcc     evenCount       ; speed (ptr3 = ptr3/2)
oddCount:
                                ; y is still 0 here
        txa                     ; restore fill value
        sta     (ptr1),y        ; save value and increase
        inc     ptr1            ; dest. pointer
        bne     evenCount
        inc     ptr1+1
evenCount:
        lda     ptr1            ; build second pointer section
        clc
        adc     ptr3            ; ptr2 = ptr1 + (length/2) <- ptr3
        sta     ptr2
        lda     ptr1+1
        adc     ptr3+1
        sta     ptr2+1

        txa                     ; restore fill value
        ldx     ptr3+1          ; Get high byte of n
        beq     .L2              ; Jump if zero

; Set 256/512 byte blocks
                                ; y is still 0 here
.L1:
        sta     (ptr1),y        ; Set byte in lower section
        sta     (ptr2),y        ; Set byte in upper section
        iny
        sta     (ptr1),y        ; Set byte in lower section
        sta     (ptr2),y        ; Set byte in upper section
        iny

        bne     .L1
        inc     ptr1+1
        inc     ptr2+1
        dex                     ; Next 256 byte block
        bne     .L1              ; Repeat if any

; Set the remaining bytes if any

.L2:     ldy     ptr3            ; Get the low byte of n
        beq     leave           ; something to set? No -> leave

.L3:     dey
        sta     (ptr1),y                ; set bytes in low
        sta     (ptr2),y                ; and high section
        bne     .L3              ; flags still up to date from dey!
leave:
	pla
	tax
	inx
	inx
	inx
	rts
;code

code	key?
    lda $c6 ; number of characters in keyboard buffer
    beq +
    lda #$ff
+   tay
    jmp %pushya%
;code

code	key
-   lda $c6
    beq -
    stx W
    jsr $e5b4
    ldx W
    ldy #0
    jmp %pushya%
;code

variable curr
: (accept)
$cc >r 0 $cc c! \ enable cursor
swap dup >r curr !
begin
 key
 dup $d = if \ cr
  2drop curr @ r> -
  space r> $cc c! \ reset cursor
  exit
 else dup $14 = if \ del
  curr @ r@ > if
   emit -1 curr +! 1+
  else drop then
 else dup $7f and $20 < if
  drop \ ignore
 else
  \ process character
  over if dup curr @ c!
   emit 1- 1 curr +!
  else drop then
 then then then
again ;

\ Using this trampoline to avoid overriding the Python accept.
code	accept ; ( addr u -- u )
	jmp	%(accept)%
;code

code	>body
	jsr	%litc%
	!byte	5	; skips jsr dodoes and code pointer
	jmp	%+%
;code

code	(?do)
	lda	LSB,x
	cmp	LSB+1,x
	bne	.enter_loop
	lda	MSB,x
	cmp	MSB+1,x
	bne	.enter_loop

	; skip loop
	inx
	inx
	jmp	%branch%

.enter_loop
	pla
	tay
	pla
	sta	W

	lda	MSB+1,x
	pha
	lda	LSB+1,x
	pha

	lda	MSB,x
	pha
	lda	LSB,x
	pha

	inx
	inx

	; ip += 2
	iny
	bne	+
	inc	W
+	iny
	bne	+
	inc	W
+
	lda	W
	pha
	tya
	pha
	rts
;code

code	(of)
	lda	LSB,x
	cmp	LSB+1,x
	bne	.endof
	lda	MSB,x
	cmp	MSB+1,x
	bne	.endof
	; enter
	inx
	inx
	jsr	%r>%
	jsr	%2+%
	jsr	%>r%
	rts
.endof	inx
	jmp	%branch%
;code

\ This is obviously not a proper QUIT, but since we do not have QUIT on C64, this is at least something.
code	quit
	jmp	%bye%
;code

code	page
	lda	#$93
	jmp	PUTCHR
;code

: environment?
2dup s" /COUNTED-STRING" s= if 2drop 255 true exit then
2dup s" /HOLD" s= if 2drop 34 true exit then \ minimum size: (2 x n) + 2 characters, where n is number of bits in a cell
2dup s" /PAD" s= if 2drop 84 true exit then \ minimum size
2dup s" ADDRESS-UNIT-BITS" s= if 2drop 8 true exit then \ 8 bits in a byte
2dup s" FLOORED" s= if 2drop true true exit then \ symmetric division considered harmful
2dup s" MAX-CHAR" s= if 2drop 255 true exit then
2dup s" MAX-D" s= if 2drop -1 $7fff true exit then
2dup s" MAX-N" s= if 2drop $7fff true exit then
2dup s" MAX-U" s= if 2drop -1 true exit then
2dup s" MAX-UD" s= if 2drop -1 -1 true exit then
2dup s" RETURN-STACK-CELLS" s= if 2drop $7a true exit then \ When entering start word, SP=$f4
2dup s" STACK-CELLS" s= if 2drop $38 true exit then
2drop false ;
