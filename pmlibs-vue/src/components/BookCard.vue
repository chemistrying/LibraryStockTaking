<template>
    <!-- <div class="card h-100 w-100 border" :class="bookCardClass" style="auto">
        <div class="card-body">
            <div class="row row-cols-1 row-cols-md-auto">
                <div class="col">
                    <p class="card-text text-secondary"> Input Time: {{ book.inputTime }} </p>
                </div>
                <div class="col" v-if="book.returnedResponse.verdict != 'error'">
                    <p class="card-text"> {{ book.returnedResponse.bookInformation }} </p>
                </div>
                <div class="col" v-if="book.returnedResponse.verdict == 'error'">
                    <p class="card-text"> {{ book.barcode }} | ??? | ??? | ??? | ??? | ??? | ??? | ??? | ??? </p>
                </div>

            </div>
            
        </div>
    </div> -->

    <button class="accordion-button" :class="Object.assign({}, bookCardClass, { collapsed: !isActive })" type="button"
        data-bs-toggle="collapse" :aria-expanded="isActive" @click.prevent="toggleMessage">
        <div class="row row-cols-1 row-cols-md-auto">
            <div class="col">
                <p class="card-text"> Input Time: {{ readableTime(book.inputTime) }} </p>
            </div>
            <div class="col" v-if="book.returnedResponse.verdict != 'error'">
                <p class="card-text"> {{ book.returnedResponse.bookInformation }} </p>
            </div>
            <div class="col" v-if="book.returnedResponse.verdict == 'error'">
                <p class="card-text"> {{ book.barcode }} | ??? | ??? | ??? | ??? | ??? | ??? | ??? | ??? </p>
            </div>
        </div>
    </button>
    <div class="accordion-collapse bg-light" :class="{ collapse: !isActive, show: isActive }"
        data-bs-parent="#accordionBooks">
        <div class="accordion-body">
            <p class="text-secondary">
                Result:
                <span class="badge rounded-pill" :class="badgeClass"> {{ book.returnedResponse.verdict.toProperCase()
                    }}</span>
            </p>
            <p class="text-secondary"> Message: {{ book.returnedResponse.message }} </p>
            <!-- <p class="text-secondary"> {{ book.returnedResponse }} </p> -->
            <button type="submit" class="d-flex h-100 align-items-center btn btn-danger"
                :disabled="bookshelf.status == 3" @click.prevent="deleteEntry">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash"
                    viewBox="0 0 16 16">
                    <path
                        d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5m2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5m3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0z" />
                    <path
                        d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4zM2.5 3h11V2h-11z" />
                </svg>
            </button>
        </div>
    </div>
</template>

<script>
String.prototype.toProperCase = function () {
    // https://stackoverflow.com/questions/196972/convert-string-to-title-case-with-javascript
    return this.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); });
};

export default {
    props: ['book', 'bookshelf', 'session'],
    computed: {
        bookCardClass() {
            console.log(this.book.returnedResponse.verdict);
            return {
                'bg-info-subtle border-info': this.book.returnedResponse.verdict == "ok",
                'bg-warning-subtle border-warning': this.book.returnedResponse.verdict == "warning",
                'bg-danger-subtle border-danger': this.book.returnedResponse.verdict == "error"
            }
        },
        badgeClass() {
            return {
                'text-bg-success': this.book.returnedResponse.verdict == "ok",
                'text-bg-warning': this.book.returnedResponse.verdict == "warning",
                'text-bg-danger': this.book.returnedResponse.verdict == "error"
            }
        }
    },
    data() {
        return {
            isActive: false
        }
    },
    methods: {
        toggleMessage() {
            this.isActive ^= 1;
        },
        readableTime(isoTime) {
            const date = new Date(isoTime);
            const dateTimeFormat = new Intl.DateTimeFormat('en-GB', {
                year: 'numeric',
                month: 'numeric',
                day: 'numeric',
                hour: "numeric",
                minute: "numeric",
                second: "numeric"
            });

            const retTime = dateTimeFormat.format(date)
            console.log(retTime);
            return retTime;
        },
        async deleteEntry() {
            var barcode = this.book.barcode;
            console.log(barcode);

            var data = {
                sessionId: this.session.id,
                bookshelfId: this.bookshelf.id,
                operation: this.$root.operations.DELETE,
                barcode: barcode,
            }

            console.log(data);

            var response = await fetch(`${this.$root.apiUrl}/api/stocktake`, {
                method: "PATCH",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(data)
            });

            console.log(response.json());

            this.$parent.$parent.refreshBookshelf();
        }
    },
    mounted() {
        // console.log(this.bookshelf.allBooks);
    }
}
</script>